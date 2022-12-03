use std::collections::HashMap;
use std::collections::HashSet;
use std::error::Error;
use std::str::FromStr;
use std::time::Instant;

macro_rules! err {
    ($($tt:tt)*) => { Err(Box::<dyn Error>::from(format!($($tt)*))) }
}

fn main() {
    part_a();
    part_b();
}
fn part_b() {
    println!("sum of factors of 864: {}", all_factors(10551264));
}
fn part_a() {
    println!("Hello, world!");
    let inp = include_str!("../input.txt");
    let mut state: State = inp.parse().unwrap();
    let res = state.execute();
    println!("res: {}", res);
}

#[derive(PartialEq, Debug)]
struct State {
    ip: usize,
    regs: Vec<usize>,
    commands: Vec<Command>,
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
            commands
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
        Ok(Command {
            opt, a, b, c
        })
    }
}

impl State {
    fn execute(&mut self) -> usize {
        while self.regs[self.ip] < self.commands.len() {
            self.step();
        }
        return self.regs[0];
    }
    fn step(&mut self) -> Vec<usize> {
        let cur = &self.commands[self.regs[self.ip]];
        if self.regs[self.ip] == 3 {
            // self.regs[5] = 16;
            println!("regs {:?}", self.regs);
        }
        // println!("command: {:?}", cur);
        // println!("executing : {:?} with regs {:?}", cur, self.regs);
        // println!("ip is {} {:?}", self.regs[self.ip], self.commands[self.regs[self.ip]]);
        let res = do_opt(cur, &self.regs);
        // println!("res is : {:?}", res);
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

fn all_factors(num: usize) -> usize {
    let mut s = 0;
    for i in 1..=(num / 2) {
        if num % i == 0 {
            s += i; 
        }
    }
    s + num
}

impl From<&str> for OptCode {
    fn from(st: &str) -> Self {
        match st {
            "addr" =>  OptCode::Addr,
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
            regs: vec![0,0,0,0,0,0],
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

    #[test]
    fn test_execute() {
        let inp = include_str!("../test.txt");
        let mut state: State = inp.parse().unwrap();
        let res = state.execute();
        assert_eq!(res, 7);
    }
    #[test]
    fn sum_factors() {
        assert_eq!(31, all_factors(16));
    }
}