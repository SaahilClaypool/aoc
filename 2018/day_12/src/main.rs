use std::fs::File;
use std::io::prelude::*;
use std::io::BufReader;
use std::mem::swap;
use std::str::FromStr;

fn main() {
    println!("Hello, world!");
    let mut input = read_input("input.txt");
    input.generations(20, true);
    println!("{}", input.state.iter().collect::<String>());
    println!("negative pots: {}", input.negative_pots);
    println!("Score: {}", input.score());
    println!("Hello, world!");

    // part 2
    // check for cyclic generations
    let mut input = read_input("input.txt");
    let score = input.generations(50000000000 as u64, true);
    println!("negative pots: {}", input.negative_pots);
    println!("Score: {}", score);

}

fn read_input(filename: &str) -> State {
    let f = File::open(filename).unwrap();
    let reader = BufReader::new(f);

    let mut start_state = State {
        state: vec![],
        rules: vec![],
        past_state: vec![],
        negative_pots: 0,
    };

    for (idx, line) in reader.lines().enumerate() {
        let line = line.unwrap();
        if idx == 0 {
            let line = line.split_whitespace().nth(2).unwrap();
            start_state.state.append(&mut line.chars().collect());
        } else if idx == 1 {
            continue;
        } else {
            start_state.rules.push(line.parse().unwrap());
        }
    }
    start_state
}

struct State {
    state: Vec<char>,
    rules: Vec<Rule>,
    past_state: Vec<(i32, Vec<char>)>,
    negative_pots: i32,
}

impl State {
    const PLANT: char = '#';
    const EMPTY: char = '.';
    fn next(&mut self) {
        let mut temp_state = vec![];
        // pots -1, -2, -3 are actually to the left of zero I guess.
        for idx in -2..(self.state.len() + 2) as i32 {
            let should_plant = self.apply_rules(idx);
            // handle planting of negatives
            if idx < 0 {
                if should_plant {
                    temp_state.push(Self::EMPTY);
                    temp_state.push(Self::EMPTY);
                    self.negative_pots += 3;
                    println!("Adding 3 pots");
                } else {
                    continue;
                }
            } else if idx > (self.state.len() - 1) as i32 && !should_plant {
                continue;
            }

            temp_state.push(if should_plant {
                Self::PLANT
            } else {
                Self::EMPTY
            });
        }
        // swap is useful because we can switch ownership
        swap(&mut self.state, &mut temp_state);
        self.past_state.push((self.negative_pots, temp_state));
    }

    /// Return true if a plant should be created on this spot
    fn apply_rules(&self, idx: i32) -> bool {
        for rule in &self.rules {
            if rule.matches(idx, &self.state) {
                return rule.create_plant;
            }
        }
        // this might not happen ever?
        return false;
    }

    fn score_state(state: &[char], zero_index: i64) -> f64 {
        println!("Scoring with zero at: {}", zero_index);
        state.iter().enumerate().map(|(pot, plant)| {
            if *plant == Self::PLANT {
                (pot as f64 - zero_index as f64) as f64
            } else {
                0 as f64
            }
        }).sum()
    }

    fn score(&self) -> f64 {
        Self::score_state(&self.state, self.negative_pots as i64)
    }

    /// matches the same state with an offset
    /// the offset is the number of shifts over the new is of the old (to the right)
    fn compare_state(old: &[char], new: &[char]) -> (bool, i32) {
        // old always smaller
        let mut first_old_plant = 0;
        let mut last_old_plant = 0;
        let mut first_new_plant = 0;
        let mut last_new_plant = 0;
        for old_idx in 0..old.len() {
            if old[old_idx] == Self::PLANT {
                if first_old_plant == 0 {
                    first_old_plant = old_idx;
                } else {
                    last_old_plant = old_idx;
                }
            }
        }
        for new_idx in 0..new.len() {
            if new[new_idx] == Self::PLANT {
                if first_new_plant == 0 {
                    first_new_plant = new_idx;
                } else {
                    last_new_plant = new_idx;
                }
            }
        }

        if new.len() - old.len() < (first_new_plant as i32 - first_old_plant as i32).abs() as usize 
            || first_new_plant < first_old_plant {
            return (false, 0);
        }

        let offset = first_new_plant as i32 - first_old_plant as i32; 

        if offset + last_old_plant as i32 != last_new_plant as i32 {
            return (false, 0);
        }

        for i in first_old_plant..old.len(){
            if i as i32 + offset >= new.len() as i32 {
                return (false, 0);
            }
            if old[i] != new[(i as i32 + offset) as usize] {
                return (false, 0);
            }
        }
        (true, offset)
    }

    /// return the score after gens generations
    /// destroys state - don't use after this
    fn generations (&mut self, gens: u64, exit_early: bool) -> f64 {
        for current_gen in 0..gens{
            self.next();
            for (orig_idx, (neg_pots, state)) in self.past_state[0..self.past_state.len() - 0].iter().enumerate() {
                let (same, shift) = Self::compare_state(state, &self.state);
                if same {
                    // we repeat every idx generations. 
                    // so, the last generations will look like
                    // the one mod[idx] after this one
                    let new = self.state.iter().collect::<String>();
                    let old = state.iter().collect::<String>();
                    println!("new: {}\nold: {}", new, old);
                    let cycle_len = current_gen - orig_idx as u64 + 1; 
                    let remaining = gens - orig_idx as u64; 
                    let offset = remaining % cycle_len;
                    let cycles_to_go = (remaining / cycle_len) as i64;
                    let (negative_pots, last_state) = &self.past_state[orig_idx + offset as usize];
                    println!("cycle len: {} remain {} shift: {} cycles to go: {}", cycle_len, remaining, shift, cycles_to_go);
                    let old_score = Self::score_state(last_state, *negative_pots as i64);
                    println!("old score: {}", old_score);
                    let total_shift = cycles_to_go * shift as i64;
                    let new_score = Self::score_state(last_state, *negative_pots as i64 - total_shift);
                    println!("new score: {}", new_score);
                    println!("old state idx: {} target: {}", orig_idx, gens);
                    if exit_early {
                        return new_score;
                    }
                }
            }
        }

        self.score()
    }
}

#[derive(Debug, PartialEq)]
struct Rule {
    /// if this rule passes, should we create a plant
    create_plant: bool,
    rule: Vec<char>,
}

impl Rule {
    fn matches(&self, idx: i32, state: &[char]) -> bool {
        let idx = idx as i32; // need to allow negatives
        for (match_index, state_index) in (idx - 2..=idx + 2).enumerate() {
            let c = if state_index < 0 || state_index > (state.len() - 1) as i32 {
                // treat the things of the edge as not existing
                '.'
            } else {
                state[state_index as usize]
            };
            if self.rule[match_index] != c {
                return false;
            }
        }
        true
    }
}

impl FromStr for Rule {
    type Err = Box<dyn std::error::Error>;

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        let mut parts = s.split_whitespace();
        let rule: Vec<char> = parts.nth(0).unwrap().chars().collect();
        let create_plant: bool = parts.nth(1).unwrap().trim() == "#";

        Ok(Rule {
            create_plant: create_plant,
            rule: rule,
        })
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_parse() {
        let input = "...## => #";
        let parsed: Rule = input.parse().unwrap();
        assert_eq!(
            parsed,
            Rule {
                create_plant: true,
                rule: vec!['.', '.', '.', '#', '#']
            }
        );
    }

    #[test]
    fn test_example() {
        let mut input = read_input("test.txt");
    }

    #[test]
    fn test_score() {
        let state = ".#....##....#####...#######....#.#..##.".chars().collect::<Vec<char>>();

        let state = State {
            state: state, 
            negative_pots: 3, 
            past_state: vec![],
            rules: vec![],
        };

        assert_eq!(325 as f64, state.score());

    }

    #[test]
    fn test_example_score() {
        let mut input = read_input("test.txt");
        eprintln!("{}", input.state.iter().collect::<String>());
        for _ in 0..20 {
            input.next();
            eprintln!("{}", input.state.iter().collect::<String>());
        }
        assert_eq!(input.state.iter().collect::<String>(), ".#....##....#####...#######....#.#..##".chars().collect::<String>());
        assert_eq!(input.score(), 325 as f64);
    }
}
