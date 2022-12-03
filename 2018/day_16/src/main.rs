use std::collections::HashMap;
use std::collections::HashSet;
use std::error::Error;
use std::str::FromStr;
use std::time::Instant;

macro_rules! err {
    ($($tt:tt)*) => { Err(Box::<dyn Error>::from(format!($($tt)*))) }
}

fn main() {
    // let input = include_str!("input.txt");
    let input = include_str!("dad.txt");
    let start = Instant::now();
    part_a(input);
    let end = Instant::now();
    println!("Finished A in : {} ms", end.duration_since(start).as_secs() * 1000 + end.duration_since(start).subsec_millis() as u64);

    let start = Instant::now();
    part_b(input);
    let end = Instant::now();
    println!("Finished B in : {} ms", end.duration_since(start).as_secs() * 1000 + end.duration_since(start).subsec_millis() as u64);
}

fn part_a(input: &str) {
    let parsed = parse_input(input);
    let count_possible: usize = parsed
        .iter()
        .filter(|sample| {
            let num_possible = sample.possible_opts().1.len();
            num_possible > 2
        })
        .count();
    eprintln!("Number of samples > 3 possible : {}", count_possible);
}

fn part_b(input: &str) {
    let parsed = parse_input(input);
    let mut rule_set: HashMap<usize, HashSet<usize>> = HashMap::new();
    let rules: Vec<(usize, Vec<usize>)> =
        parsed.iter().map(|sample| sample.possible_opts()).collect();
    // for each opt paired with the possible mappings, add them all to a hashmap
    // though, this would make more sense if it found the minimum subset of the rules... oh well
    for (opt, possible) in rules {
        let entry = rule_set.entry(opt).or_default();
        for pos in possible {
            entry.insert(pos);
        }
    }

    let mut finished: Vec<usize> = vec![];

    'outer: loop {
        let mut to_remove = 0;
        'inner: for (opt, possible) in rule_set.iter() {
            if possible.len() == 1 && !finished.contains(&opt) {
                // only one rule, remove it from all the others
                to_remove = *opt;
                finished.push(*opt);
                break 'inner;
            }
        }
        let opt_to_remove = *rule_set[&to_remove].iter().nth(0).unwrap();
        for (opt, possible) in rule_set.iter_mut() {
            if *opt != to_remove {
                possible.remove(&opt_to_remove);
            }
        }
        let all_one = rule_set
            .iter()
            .filter(|(_opt, possible)| possible.len() > 1)
            .count()
            == 0;
        if all_one {
            break 'outer;
        }
    }

    let rule_map: HashMap<usize, usize> = rule_set
        .iter()
        .map(|(opt, possible)| (*opt, *possible.iter().nth(0).unwrap()))
        .collect();

    let lines: Vec<&str> = input.lines().collect();

    // let test_prog: Vec<Inst> = lines[3187..]
    let test_prog: Vec<Inst> = lines[3131..]
        .iter()
        .map(|line| Inst::from_str(line).unwrap())
        .collect();
    println!("Rules Map: {:#?}", rule_map);
    let mut state = vec![0, 0, 0, 0];
    for inst in test_prog {
        state = do_opt(OptCode::from(rule_map[&inst.code]), &inst, &state);
    }

    println!("reg 0 is {}", state[0]);

}

fn parse_input(input: &str) -> Vec<Sample> {
    let mut samples = vec![];
    let lines: Vec<&str> = input.lines().collect();
    'chunker: for line in lines.chunks(4) {
        let sample = line.join("\n");
        let sample = Sample::from_str(&sample);
        match sample {
            Ok(sample) => samples.push(sample),
            Err(err) => {
                // eprintln!("{:?}", err.to_string());
                break 'chunker;
            }
        }
    }
    samples
}

type RegMap = HashMap<usize, OptCode>;

#[derive(Debug, PartialEq)]
struct Sample {
    before: Vec<usize>,
    after: Vec<usize>,
    inst: Inst,
}

impl Sample {
    // get the possible opts for this single sample
    fn possible_opts(&self) -> (usize, Vec<usize>) {
        let mut opts = vec![];
        for i in 0..16 {
            if do_opt(OptCode::from(i), &self.inst, &self.before) == self.after {
                opts.push(i);
            }
        }
        (self.inst.code, opts)
    }
}
fn do_opt(opt: OptCode, inst: &Inst, before: &[usize]) -> Vec<usize> {
    let mut out: Vec<usize> = before.iter().map(|el| *el).collect();
    match opt {
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
            let _b = before[inst.b];
            out[inst.c] = a;
        }
        OptCode::Seti => {
            let a = inst.a;
            let _b = before[inst.b];
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

impl FromStr for Sample {
    type Err = Box<dyn std::error::Error>;

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        let lines: Vec<&str> = s.lines().collect();
        if lines.len() < 3 || lines[0].len() < 2 {
            return err!("Bad input for sample: {:?}", lines);
        }
        let before_slice = &lines[0][9..lines[0].len() - 1];
        let before = before_slice
            .split(", ")
            .map(|st| st.parse::<usize>().unwrap())
            .collect();

        let after_slice = &lines[2][9..lines[2].len() - 1];
        let after = after_slice
            .split(", ")
            .map(|st| st.parse::<usize>().unwrap())
            .collect();

        let inst = lines[1].parse().unwrap();

        Ok(Self {
            before,
            after,
            inst,
        })
    }
}

#[derive(Debug, PartialEq)]
struct Inst {
    code: usize,
    a: usize, // input a
    b: usize, // input b
    c: usize,
}

impl FromStr for Inst {
    type Err = Box<dyn std::error::Error>;

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        let nums: Vec<usize> = s.split(" ").map(|st| st.parse().unwrap()).collect();
        Ok(Inst {
            code: nums[0],
            a: nums[1],
            b: nums[2],
            c: nums[3],
        })
    }
}

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

impl From<usize> for OptCode {
    fn from(t: usize) -> OptCode {
        match t {
            0 => OptCode::Addr,
            1 => OptCode::Addi,
            2 => OptCode::Mulr,
            3 => OptCode::Muli,
            4 => OptCode::Banr,
            5 => OptCode::Bani,
            6 => OptCode::Borr,
            7 => OptCode::Bori,
            8 => OptCode::Setr,
            9 => OptCode::Seti,
            10 => OptCode::Gtir,
            11 => OptCode::Gtri,
            12 => OptCode::Gtrr,
            13 => OptCode::Eqir,
            14 => OptCode::Eqri,
            15 => OptCode::Eqrr,
            _ => OptCode::Addr,
        }
    }
}

impl From<OptCode> for usize {
    fn from(t: OptCode) -> usize {
        match t {
            OptCode::Addr => 0,
            OptCode::Addi => 1,
            OptCode::Mulr => 2,
            OptCode::Muli => 3,
            OptCode::Banr => 4,
            OptCode::Bani => 5,
            OptCode::Borr => 6,
            OptCode::Bori => 7,
            OptCode::Setr => 8,
            OptCode::Seti => 9,
            OptCode::Gtir => 10,
            OptCode::Gtri => 11,
            OptCode::Gtrr => 12,
            OptCode::Eqir => 13,
            OptCode::Eqri => 14,
            OptCode::Eqrr => 15,
            _ => 0,
        }
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_parse() {
        let input = "Before: [1, 1, 1, 0]
4 1 0 0
After:  [1, 1, 1, 0]";
        let sample: Sample = input.parse().unwrap();
        assert_eq!(
            sample,
            Sample {
                before: vec![1, 1, 1, 0],
                after: vec![1, 1, 1, 0],
                inst: Inst {
                    code: 4,
                    a: 1,
                    b: 0,
                    c: 0
                }
            }
        );
    }

    #[test]
    fn test_add_mul() {
        let input = "Before: [3, 2, 1, 1]
9 2 1 2
After:  [3, 2, 2, 1]";
        let sample: Sample = input.parse().unwrap();
        assert_eq!(do_opt(OptCode::Mulr, &sample.inst, &sample.before), sample.after);
        assert_eq!(do_opt(OptCode::Addi, &sample.inst, &sample.before), sample.after);
        assert_eq!(do_opt(OptCode::Seti, &sample.inst, &sample.before), sample.after);
    }
    #[test]
    fn test_possible() {
        let input = "Before: [3, 2, 1, 1]
9 2 1 2
After:  [3, 2, 2, 1]";
        let sample: Sample = input.parse().unwrap();

        let (opt, possible) = sample.possible_opts();
        assert_eq!(
            possible,
            vec![
                usize::from(OptCode::Addi),
                usize::from(OptCode::Mulr),
                usize::from(OptCode::Seti)
            ]
        )
    }
}
