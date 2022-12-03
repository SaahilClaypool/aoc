use std::error::Error;
use std::io::BufReader;
use std::io::prelude::*;
use std::collections::HashSet;
use std::fmt::{Debug, Display};

type Res<T> = Result<T, Box<dyn Error>>;

fn part1() -> Res<()> {
    println!("Hello, world!");
    let f = std::fs::File::open("input_a.txt")?;
    let r = BufReader::new(f);
    let mut sum = 0;
    for line in r.lines() {
        let line = line?;
        let sign = match line.chars().nth(0) {
            Some('-') => -1, 
            Some('+') => 1,
            _ => 1
        };
        let rest = &line[1..];
        let val = rest.to_string().parse::<i32>()?;
        sum += val * sign;
    }
    println!("Sum is {}", sum);
    Ok(())
}

fn part2(mut sum: i32, mut set: HashSet<i32>) -> Res<i32> {
    let f = std::fs::File::open("input_a.txt")?;
    let r = BufReader::new(f);
    set.insert(sum);
    for line in r.lines() {
        let line = line?;
        let line = line.trim();
        let sign = match line.chars().nth(0) {
            Some('-') => -1, 
            Some('+') => 1,
            _ => 1
        };
        let rest = &line[1..];
        let val = rest.to_string().parse::<i32>()?;
        sum += val * sign;
        if set.contains(&sum) {
            return Ok(sum);
        }
        else {
            set.insert(sum);
        }
    }
    return part2(sum, set);
}

fn main() -> Res<()> {
    part1()?;
    let res = part2(0, HashSet::new())?;
    println!("part 2: {}", res);
    Ok(())
}
