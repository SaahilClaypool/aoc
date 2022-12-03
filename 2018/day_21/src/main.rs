use std::collections::HashMap;
use std::collections::HashSet;
use std::error::Error;
use std::str::FromStr;
use std::time::Instant;

mod part2;
use crate::part2::do_part_2;

macro_rules! err {
    ($($tt:tt)*) => { Err(Box::<dyn Error>::from(format!($($tt)*))) }
}

fn main() {
    max_halting();
    // let o = lowest_halting(7434231);
    // println!("o is : {}", o);
    // do_part_2(11513432);
    // let mut max_count = 0;
    // let mut max_num = 0;
    // for i in 1..11513432 * 2 {
    //     let (c, num) = part2::do_part_2(i);
    //     if c > max_count {
    //         max_count = c;
    //         max_num = num;
    //         eprintln!("max num: {} max_count: {}", max_num, max_count);
    //     }
    // }
    // eprintln!("max num: {} max_count: {}", max_num, max_count);
    // part_b();
}
fn part_b() {
    let l = lowest_halting(11513432);
}
fn part_a() {
    let l = lowest_halting(11513432);
    println!("lowest halting is {}", l);
}

#[derive(PartialEq, Debug)]
struct State {
    ip: usize,
    regs: Vec<usize>,
    commands: Vec<Command>,
    active: bool,
    count_map: HashMap<usize, usize>,
}

#[derive(PartialEq, Debug)]
struct Command {
    opt: OptCode,
    a: usize,
    b: usize,
    c: usize,
}

impl FromStr for State {
    type Err = Box<dyn std::error::Error>;
    fn from_str(s: &str) -> Result<Self, Self::Err> {
        let mut lines = s.lines();
        let reg: usize = lines.nth(0).unwrap()[4..].parse().unwrap();
        let commands: Vec<Command> = lines.map(|line| line.parse().unwrap()).collect();
        Ok(State {
            ip: reg,
            regs: vec![0, 0, 0, 0, 0, 0],
            active: true,
            count_map: HashMap::new(),
            commands,
        })
    }
}

impl FromStr for Command {
    type Err = Box<dyn std::error::Error>;
    fn from_str(s: &str) -> Result<Self, Self::Err> {
        let mut parts = s.split_whitespace();
        let opt = OptCode::from(parts.nth(0).unwrap());
        let a: usize = parts.nth(0).unwrap().parse()?;
        let b: usize = parts.nth(0).unwrap().parse()?;
        let c: usize = parts.nth(0).unwrap().parse()?;
        Ok(Command { opt, a, b, c })
    }
}

impl ToString for Command {
    fn to_string(&self) -> String {
        let fmt = match self.opt {
            OptCode::Addr => "r{c} = r{a} + r{b}",
            OptCode::Addi => "r{c} = r{a} + {b}",
            OptCode::Mulr => "r{c} = r{a} * r{b}",
            OptCode::Muli => "r{c} = r{a} * {b}",
            OptCode::Banr => "r{c} = r{a} & r{b}",
            OptCode::Bani => "r{c} = r{a} & {b}",
            OptCode::Borr => "r{c} = r{a} | r{b}",
            OptCode::Bori => "r{c} = r{a} | {b}",
            OptCode::Setr => "r{c} = r{a}",
            OptCode::Seti => "r{c} = {a}",
            OptCode::Gtir => "if {a} > r{b} => r{c}",
            OptCode::Gtri => "if r{a} > {b} => r{c}",
            OptCode::Gtrr => "if r{a} > r{b} => r{c}",
            OptCode::Eqir => "if {a} == r{b} => r{c}",
            OptCode::Eqri => "if r{a} == {b} => r{c}",
            OptCode::Eqrr => "if r{a} == r{b} => r{c}",
        };
        String::from(fmt)
            .replace("{a}", &self.a.to_string())
            .replace("{b}", &self.b.to_string())
            .replace("{c}", &self.c.to_string())
    }
}

fn max_halting() {
    // this always loops
    let mut state: State = include_str!("../input2.txt").parse().unwrap();
    let mut seen = HashSet::new();
    let mut pairs = HashSet::new();
    loop {
        state.step();
        // the regs 5 and 4 control the loop
        const check_at: usize = 29;
        if state.regs[state.ip] == check_at {
            let reg5 = state.regs[5];
            let reg4 = state.regs[4];
            let reg2 = state.regs[2];
            if !seen.contains(&reg5) {
                println!("r5 is {}", reg5);
                seen.insert(reg5);
            }
            if !pairs.contains(&(reg4, reg5)) {
                pairs.insert((reg4, reg5));
            } else {
                return;
            }
        }
    }
}
fn lowest_halting(t: usize) -> usize {
    let mut states = vec![];
    let mut s: State = include_str!("../input.txt").parse().unwrap();
    s.regs[0] = t;
    states.push(s);
    for i in 0..100000000 {
        for (reg0, state) in states.iter_mut().enumerate() {
            if state.regs[state.ip] == 28 {}
            if state.regs[state.ip] >= state.commands.len() {
                print_order(&state);
                return state.regs[0];
            }
            state.step();
        }
    }
    print_order(states.iter().nth(0).unwrap());
    0
}

fn print_order(state: &State) {
    let count_map: HashMap<usize, f64> = state
        .count_map
        .iter()
        .map(|(cmd, count)| (*cmd, (*count as f64).log(2.0)))
        .collect();
    for idx in 0..state.commands.len() {
        if count_map.contains_key(&idx) {
            let count = count_map[&idx];
            print!("{}: ", idx + 1);
            for _tabs in 0..count as usize {
                print!(" ");
            }
            if idx < state.commands.len() {
                println!("{}", state.commands[idx].to_string());
            }
        }
    }
}

impl State {
    // return count, and the value of reg 0
    fn execute(&mut self) -> (usize, usize) {
        let mut count = 0;
        while self.regs[self.ip] < self.commands.len() {
            self.step();
            count += 1;
        }
        return (count, self.regs[0]);
    }
    fn step(&mut self) -> Vec<usize> {
        // println!("{:?}", self.regs);
        *self.count_map.entry(self.regs[self.ip]).or_insert(0) += 1;
        let cur = &self.commands[self.regs[self.ip]];
        let res = do_opt(cur, &self.regs);
        self.regs = res.clone();
        self.regs[self.ip as usize] += 1;
        res
    }
}

fn do_opt(inst: &Command, before: &[usize]) -> Vec<usize> {
    let mut out: Vec<usize> = before.iter().map(|el| *el).collect();
    match inst.opt {
        OptCode::Addr => {
            let a = before[inst.a];
            let b = before[inst.b];
            out[inst.c] = a + b;
        }
        OptCode::Addi => {
            let a = before[inst.a];
            let b = inst.b;
            out[inst.c] = a + b;
        }
        OptCode::Mulr => {
            let a = before[inst.a];
            let b = before[inst.b];
            // eprintln!("MULR a: {} b: {} set c: {} to {}", a, b, inst.c, a * b);
            out[inst.c] = a * b;
        }
        OptCode::Muli => {
            let a = before[inst.a];
            let b = inst.b;
            out[inst.c] = a * b;
        }
        OptCode::Banr => {
            let a = before[inst.a];
            let b = before[inst.b];
            out[inst.c] = a & b;
        }
        OptCode::Bani => {
            let a = before[inst.a];
            let b = inst.b;
            out[inst.c] = a & b;
        }
        OptCode::Borr => {
            let a = before[inst.a];
            let b = before[inst.b];
            out[inst.c] = a | b;
        }
        OptCode::Bori => {
            let a = before[inst.a];
            let b = inst.b;
            out[inst.c] = a | b;
        }
        OptCode::Setr => {
            let a = before[inst.a];
            // let _b = before[inst.b];
            out[inst.c] = a;
        }
        OptCode::Seti => {
            let a = inst.a;
            // let _b = before[inst.b];
            out[inst.c] = a;
        }
        OptCode::Gtir => {
            let a = inst.a;
            let b = before[inst.b];
            let res = if a > b { 1 } else { 0 };
            out[inst.c] = res;
        }
        OptCode::Gtri => {
            let a = before[inst.a];
            let b = inst.b;
            let res = if a > b { 1 } else { 0 };
            out[inst.c] = res;
        }
        OptCode::Gtrr => {
            let a = before[inst.a];
            let b = before[inst.b];
            let res = if a > b { 1 } else { 0 };
            out[inst.c] = res;
        }
        OptCode::Eqir => {
            let a = inst.a;
            let b = before[inst.b];
            let res = if a == b { 1 } else { 0 };
            out[inst.c] = res;
        }
        OptCode::Eqri => {
            let a = before[inst.a];
            let b = inst.b;
            let res = if a == b { 1 } else { 0 };
            out[inst.c] = res;
        }
        OptCode::Eqrr => {
            let a = before[inst.a];
            let b = before[inst.b];
            let res = if a == b { 1 } else { 0 };
            out[inst.c] = res;
        }
    };
    out
}

#[derive(PartialEq, Debug)]
enum OptCode {
    Addr,
    Addi,
    Mulr,
    Muli,
    Banr,
    Bani,
    Borr,
    Bori,
    Setr,
    Seti,
    Gtir,
    Gtri,
    Gtrr,
    Eqir,
    Eqri,
    Eqrr,
}

impl From<&str> for OptCode {
    fn from(st: &str) -> Self {
        match st {
            "addr" => OptCode::Addr,
            "addi" => OptCode::Addi,
            "mulr" => OptCode::Mulr,
            "muli" => OptCode::Muli,
            "banr" => OptCode::Banr,
            "bani" => OptCode::Bani,
            "borr" => OptCode::Borr,
            "bori" => OptCode::Bori,
            "setr" => OptCode::Setr,
            "seti" => OptCode::Seti,
            "gtir" => OptCode::Gtir,
            "gtri" => OptCode::Gtri,
            "gtrr" => OptCode::Gtrr,
            "eqir" => OptCode::Eqir,
            "eqri" => OptCode::Eqri,
            "eqrr" => OptCode::Eqrr,
            _ => panic!("unknown optcode: {}", st),
        }
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    #[test]
    fn test() {
        let state = State {
            ip: 0,
            regs: vec![0, 0, 0, 0, 0, 0],
            commands: vec![
                "seti 5 0 1".parse().unwrap(),
                "seti 6 0 2".parse().unwrap(),
                "addi 0 1 0".parse().unwrap(),
                "addr 1 2 3".parse().unwrap(),
                "setr 1 0 0".parse().unwrap(),
                "seti 8 0 4".parse().unwrap(),
                "seti 9 0 5".parse().unwrap(),
            ],
        };
        let inp = include_str!("../test.txt");
        let state_parsed: State = inp.parse().unwrap();
        assert_eq!(state, state_parsed)
    }

    #[test]
    fn test_some_input() {
        let inp = include_str!("../test.txt");
        let mut state: State = inp.parse().unwrap();
        let res1 = state.step();
        assert_eq!(res1, vec![0, 5, 0, 0, 0, 0]);
        let res2 = state.step();
        assert_eq!(res2, vec![1, 5, 6, 0, 0, 0]);
    }
}
