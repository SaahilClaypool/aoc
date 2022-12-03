use std::{
    cmp::Ordering,
    collections::{HashMap, HashSet},
    str::FromStr,
};

#[macro_use]
extern crate log;

use lazy_static::lazy_static;
use regex::Regex;

fn main() {
    env_logger::init();
    // part_1();
    part_2();
}

fn part_1() {
    let mut state: State = include_str!("../input.txt").parse().unwrap();
    println!("{:#?}", state);
    state.run();
}

fn part_2() {
    let state: State = include_str!("../input.txt").parse().unwrap();
    println!("{:#?}", state);
    let (boost, rem) = find_boost(&state);
    println!("requires {} to win with {} units", boost, rem);
}

#[derive(Debug, Clone)]
struct State {
    groups: Vec<Group>,
}

// smallest (boost, units left)
fn find_boost(state: &State) -> (i32, i32) {
    let mut boost = 0;
    loop {
        let mut temp_state = state.clone();
        for group in temp_state.groups.iter_mut() {
            if group.team == "immune" {
                group.attack_dmg += boost;
            }
        }
        let (immune_won, units) = temp_state.run();
        if immune_won {
            return (boost, units);
        }
        boost += 1;
    }
}

impl State {
    fn run(&mut self) -> (bool, i32) {
        loop {
            let immune_units: i32 = self
                .groups
                .iter()
                .filter(|group| group.team == "immune")
                .map(|group| group.units)
                .sum();

            let infection_units: i32 = self
                .groups
                .iter()
                .filter(|group| group.team == "infection")
                .map(|group| group.units)
                .sum();

            if immune_units == 0 {
                println!("infection wins with: {} units", infection_units);
                return (false, infection_units);
            } else if infection_units == 0 {
                self.groups
                    .iter()
                    .filter(|group| group.team == "immune")
                    .for_each(|group| println!("immune: {}", group.units));
                println!("immune wins with: {} units", immune_units);
                return (true, immune_units);
            }
            let infinite_loop = self.round();
            if infinite_loop {
                println!("Tie");
                return (false, 0);
            }
        }
    }
    /// return true on an infinite loop
    fn round(&mut self) -> bool {
        let selections = self.selection();
        return self.attack(selections);
    }

    /// sort by initiative
    /// deal damage to selection based on power. 0 if immune. Double if weak
    /// return true if there is an infinite loop
    fn attack(&mut self, selections: HashMap<usize, usize>) -> bool {
        info!("ATTACK\n");
        let mut ids: Vec<usize> = (0..self.groups.len()).collect();
        ids.sort_by(|a, b| {
            let a = &self.groups[*a];
            let b = &self.groups[*b];
            b.initiative.cmp(&a.initiative)
        });
        let mut some_lost = false;
        for id in ids {
            if !self.groups[id].alive {
                continue;
            }
            let target_id = match selections.get(&id) {
                Some(target_id) => *target_id,
                None => continue,
            };
            let damage = self.groups[id].damage(&self.groups[target_id]);
            let target = &mut self.groups[target_id];
            let lost = target.lose_units(damage);
            if lost > 0 {
                some_lost = true;
            }
            info!(
                "{} {} attacks {} {} for {}, loses {}",
                self.groups[id].team, id, self.groups[target_id].team, target_id, damage, lost
            );
        }
        return !some_lost;
    }

    /// return the selection for each group
    fn selection(&self) -> HashMap<usize, usize> {
        let mut ids: Vec<usize> = (0..self.groups.len()).collect();
        ids.sort_by(|a, b| {
            let a = &self.groups[*a];
            let b = &self.groups[*b];
            a.cmp(&b)
        });
        let mut selected_targets = HashSet::new();
        let mut selections = HashMap::new();
        for idx in ids {
            let group = &self.groups[idx];
            let idx = group.id;
            if !group.alive {
                continue;
            }
            let targets: Vec<&Group> = self
                .groups
                .iter()
                .filter(|other| {
                    other.team != group.team && !selected_targets.contains(&other.id) && other.alive
                })
                .collect();
            if targets.is_empty() {
                continue;
            }
            let target = group.select(&targets);
            if group.damage(&self.groups[target]) <= 0 {
                continue;
            }
            selected_targets.insert(target);
            selections.insert(idx, target);
        }
        selections
    }
}

#[derive(Eq, PartialEq, Debug, Clone)]
struct Group {
    units: i32,
    health: i32,
    weak: Vec<String>,
    immune: Vec<String>,
    attack_dmg: i32,
    attack_type: String,
    initiative: i32,
    id: usize,
    alive: bool,
    team: String,
}

impl Group {
    fn power(&self) -> i32 {
        self.units * self.attack_dmg
    }
    // get the best target to fight
    fn select<'a>(&self, others: &[&'a Group]) -> usize {
        others
            .iter()
            .fold(
                (0, 0, 0, 0),
                |(best_idx, best_dmg, best_pwr, best_init), other| {
                    let pot_dmg = self.damage(other);
                    let pot_pwr = other.power();
                    let pot_init = other.initiative;
                    info!(
                        "{} group {} would do {} damage to {} group {}",
                        self.team, self.id, pot_dmg, other.team, other.id
                    );
                    if other.alive
                        && (pot_dmg > best_dmg
                            || (pot_dmg == best_dmg && pot_pwr > best_pwr)
                            || (pot_dmg == best_dmg && pot_pwr == best_pwr && pot_init > best_init))
                    {
                        (other.id, pot_dmg, pot_pwr, pot_init)
                    } else {
                        (best_idx, best_dmg, best_pwr, best_init)
                    }
                },
            )
            .0
    }

    fn damage(&self, other: &Self) -> i32 {
        if self.units <= 0 {
            return 0;
        }
        let mut dmg = self.power();
        if other.weak.contains(&self.attack_type) {
            dmg *= 2;
        } else if other.immune.contains(&self.attack_type) {
            dmg = 0;
        }
        dmg
    }

    /// lose units equal to the damage / hitpoints
    fn lose_units(&mut self, damage: i32) -> i32 {
        if damage == 0 {
            return 0;
        }
        let lost = damage / self.health;
        let lost = std::cmp::min(lost, self.units);
        self.units -= lost;
        if self.units < 1 {
            self.alive = false;
        }
        lost
    }

    /// return vec of immune, weakness
    fn parse_weaknesses(st: String) -> (Vec<String>, Vec<String>) {
        let mut immune = vec![];
        let mut weak = vec![];
        let groups = st.split(";");
        info!("weaknesses: {:?}", groups);
        for group in groups {
            if group.contains("immune to ") {
                const cutoff: usize = 10;
                let group = &group[cutoff..];
                for immunity in group.split(",") {
                    let immunity = immunity.trim();
                    immune.push(String::from(immunity));
                }
            } else if group.contains("weak to ") {
                const cutoff: usize = 8;
                let group = &group[cutoff..];
                for weakness in group.split(",") {
                    let weakness = weakness.trim();
                    weak.push(String::from(weakness));
                }
            }
        }
        (immune, weak)
    }
}

impl Ord for Group {
    fn cmp(&self, other: &Self) -> Ordering {
        if self.power() > other.power() {
            Ordering::Less
        } else if self.power() < other.power() {
            Ordering::Greater
        } else {
            other.initiative.cmp(&self.initiative)
        }
    }
}

impl PartialOrd for Group {
    fn partial_cmp(&self, other: &Self) -> Option<Ordering> {
        Some(self.cmp(other))
    }
}

/// ID and team must be changed before running
impl FromStr for Group {
    type Err = Box<dyn std::error::Error>;
    fn from_str(s: &str) -> Result<Self, Self::Err> {
        println!("parsing: {}", s);
        lazy_static! {
            static ref RE: Regex = Regex::new(r"(?P<units>\d*) units each with (?P<hp>[^\s]+) hit points (?P<weaknesses>(\([^\)]+\) )?)with an attack that does (?P<dmg>\d*) (?P<type>[^\s]+) damage at initiative (?P<init>\d*)").unwrap();
        }
        let caps = RE.captures(s).unwrap();
        let units: i32 = caps.name("units").unwrap().as_str().parse().unwrap();
        let hp: i32 = caps.name("hp").unwrap().as_str().parse().unwrap();
        let weaknesses: String = match caps.name("weaknesses") {
            Some(weaknesses) => String::from(weaknesses.as_str())
                .replace("(", "")
                .replace(")", ""),
            None => String::new(),
        };
        let (immune, weak) = Group::parse_weaknesses(weaknesses);
        let dmg: i32 = caps.name("dmg").unwrap().as_str().parse().unwrap();
        let dmg_type: String = caps.name("type").unwrap().as_str().parse().unwrap();
        let init: i32 = caps.name("init").unwrap().as_str().parse().unwrap();
        Ok(Group {
            alive: true,
            id: 0,
            team: String::from(""),
            attack_dmg: dmg,
            attack_type: dmg_type,
            health: hp,
            units: units,
            initiative: init,
            immune: immune,
            weak: weak,
        })
    }
}

impl FromStr for State {
    type Err = Box<dyn std::error::Error>;

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        let mut is_immune = true;
        let mut skip_first = true;

        let mut units = vec![];
        let mut id = 0;
        for line in s.lines() {
            if skip_first {
                skip_first = false;
                continue;
            }
            if line.len() < 2 {
                is_immune = false;
                skip_first = true;
                continue;
            }

            let mut g: Group = line.parse().unwrap();
            if !is_immune {
                g.team = String::from("infection");
            } else {
                g.team = String::from("immune");
            }
            g.id = id;
            id += 1;
            units.push(g);
        }

        Ok(State { groups: units })
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    #[test]
    fn test() {
        let g: Group = "2987 units each with 5418 hit points (immune to slashing; weak to cold, bludgeoning) with an attack that does 17 cold damage at initiative 5".parse().unwrap();
        println!("{:#?}", g);
        let g: Group = "1980 units each with 9978 hit points (immune to cold) with an attack that does 47 cold damage at initiative 19".parse().unwrap();
        println!("{:#?}", g);
        let g: Group = "949 units each with 3117 hit points with an attack that does 29 fire damage at initiative 10".parse().unwrap();
        println!("{:#?}", g);
    }

    #[test]
    fn test_sample() {
        let state: State = include_str!("../test.txt").parse().unwrap();
        println!("{:#?}", state)
    }
    #[test]
    fn test_sample_run() {
        env_logger::init();
        let mut state: State = include_str!("../test.txt").parse().unwrap();
        let (immune_won, res) = state.run();
        assert_eq!(res, 5216);
    }
    #[test]
    fn test_boost() {
        let mut state: State = include_str!("../test.txt").parse().unwrap();
        let (_, res) = find_boost(&state);
        assert_eq!(res, 51);
    }
}
