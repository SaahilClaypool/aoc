use std::str::FromStr;
const SIZE: usize = 50;

#[derive(Debug, Clone)]
struct State {
    map: Vec<Vec<Type>>,
    prev_state: Vec<Vec<Vec<Type>>>,
    round: usize,
}

impl State {
    fn score_state(state: &[Vec<Type>]) -> usize {
        let lumber: usize = state
            .iter()
            .map(|row| row.iter().filter(|t| (**t) == Type::lumber).count())
            .sum();
        let trees: usize = state
            .iter()
            .map(|row| row.iter().filter(|t| (**t) == Type::trees).count())
            .sum();

        lumber * trees
    }
    fn score(&self) -> usize {
        Self::score_state(&self.map)
    }
    fn next(&self, row: usize, col: usize) -> Type {
        match &self.map[row][col] {
            Type::trees => {
                if self.adjacent(row, col, Type::lumber) >= 3 {
                    Type::lumber
                } else {
                    Type::trees
                }
            }
            Type::lumber => {
                if self.adjacent(row, col, Type::lumber) >= 1
                    && self.adjacent(row, col, Type::trees) >= 1
                {
                    Type::lumber
                } else {
                    Type::ground
                }
            }
            Type::ground => {
                if self.adjacent(row, col, Type::trees) >= 3 {
                    Type::trees
                } else {
                    Type::ground
                }
            }
        }
    }

    fn adjacent(&self, row: usize, col: usize, t: Type) -> usize {
        let row = row as i32;
        let col = col as i32;
        let mut count = 0;
        for r in row - 1..=row + 1 {
            for c in col - 1..=col + 1 {
                if !(r == row && c == col)
                    && r >= 0
                    && r < self.map.len() as i32
                    && c >= 0
                    && c < self.map[0].len() as i32
                {
                    if self.map[r as usize][c as usize] == t {
                        count += 1;
                    }
                }
            }
        }
        count
    }

    fn step_to_target(&self, target: usize) -> usize {
        let mut current = self.clone();
        let mut cycle_len = 0;
        loop {
            current = current.step();
            if current.prev_state.contains(&current.map) {
                break;
            }
        }
        let (idx, _) = current
            .prev_state
            .iter()
            .enumerate()
            .filter(|(idx, other)| other == &&current.map)
            .nth(0)
            .unwrap();
        let cycle_len = current.round - idx;
        println!("cycle len: {}", cycle_len);
        let remaining = (target - current.round);
        println!("remaining : {}", remaining);
        let remaining = remaining % cycle_len;
        println!("mod : {}", remaining);
        let score = Self::score_state(&current.prev_state[idx + remaining]);
        println!("score: {}", score);
        score
    }

    fn step(&self) -> State {
        let cur = self.clone();
        let mut next = self.clone();
        next.round += 1;
        next.prev_state.push(cur.map);
        for (r, row) in self.map.iter().enumerate() {
            for (c, col) in row.iter().enumerate() {
                next.map[r][c] = self.next(r, c);
            }
        }
        next
    }
}

impl FromStr for State {
    type Err = Box<dyn std::error::Error>;

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        let mut map = Vec::with_capacity(SIZE);
        for line in s.lines() {
            let mut row = Vec::with_capacity(SIZE);
            for c in line.chars() {
                row.push(Type::from(c));
            }
            map.push(row);
        }
        Ok(State {
            map,
            prev_state: vec![],
            round: 0,
        })
    }
}

impl ToString for State {
    fn to_string(&self) -> String {
        let mut st = String::with_capacity(SIZE * SIZE * 2);
        for row in &self.map {
            for col in row {
                st.push(char::from(col));
            }
            st.push('\n');
        }
        String::from(st.trim_end())
    }
}

#[derive(Copy, Clone, Debug, PartialEq)]
enum Type {
    ground,
    trees,
    lumber,
}

impl From<&Type> for char {
    fn from(t: &Type) -> char {
        match t {
            Type::ground => '.',
            Type::trees => '|',
            Type::lumber => '#',
        }
    }
}

impl From<char> for Type {
    fn from(c: char) -> Type {
        match c {
            '.' => Type::ground,
            '|' => Type::trees,
            '#' => Type::lumber,
            _ => panic!("I can't handle '{}'", c),
        }
    }
}
fn part_a() {
    let test = include_str!("../input.txt");
    let mut input: State = test.parse().unwrap();
    for _ in 0..10 {
        input = input.step();
    }
    eprintln!("{}", input.to_string());
    println!("score is {}", input.score());
}

fn part_b() {
    let test = include_str!("../input.txt");
    let mut input: State = test.parse().unwrap();
    let res = input.step_to_target(1000000000);
    println!("result is {}", res)
}
fn main() {
    part_a();
    part_b();
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_parse() {
        let test = include_str!("../test.txt");
        let input: State = test.parse().unwrap();
        eprintln!("{}", input.to_string());
        assert_eq!(test, input.to_string());
    }
    #[test]
    fn test_step() {
        let test = include_str!("../test.txt");
        let step = include_str!("../step_1.txt");
        let input: State = test.parse().unwrap();
        let input = input.step();
        eprintln!("{}", input.to_string());
        assert_eq!(step, input.to_string());
    }
    #[test]
    fn test_score() {
        let test = include_str!("../test.txt");
        let mut input: State = test.parse().unwrap();
        for _ in 0..10 {
            input = input.step();
        }
        eprintln!("{}", input.to_string());
        assert_eq!(1147, input.score());
    }
}
