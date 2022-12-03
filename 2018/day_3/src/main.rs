#![feature(duration_as_u128)]
extern crate regex;
#[macro_use] extern crate simple_error;
#[macro_use] extern crate lazy_static;

use regex::Regex;
use std::error::Error;
use std::io::{prelude::*, BufReader};
use std::str::FromStr;
use std::time::Instant;

fn main() {
    let start = Instant::now();
    let claims = get_claims("input.txt");
    let fab_map = map_claims(&claims);
    let num = count_overlapping(&fab_map);
    let id = doesnt_overlap(&claims, &fab_map).unwrap();
    let end = Instant::now();
    println!("overlapping: {}", num);
    println!("id of doesn't overlap: {}", id);

    println!("Finished in : {} ms", end.duration_since(start).as_millis());
}

/// return the id of the claim that doesn't overlap
/// that is, all the squares are equal to 1
/// Note: the impl std ops index says that something must implement the index operator for a usize
/// and give back a u32. This is more simply just a Vec<Vec<u32>>, but by saying
/// its a &[impl Index<>] we can have any slice of any thing that we can index into...
/// basically just an interseting language feature
fn doesnt_overlap(
    claims: &[Claim],
    fab_map: &[impl std::ops::Index<usize, Output = u32>],
) -> Option<u32> {
    'claim_loop: for claim in claims {
        for r in claim.top..claim.top + claim.height {
            for c in claim.left..claim.left + claim.width {
                if fab_map[r as usize][c as usize] != 1 {
                    continue 'claim_loop;
                }
            }
        }
        return Some(claim.id);
    }
    None
}

/// Idea: just construct 2d vector.
/// Count sum of ones
fn map_claims(claims: &[Claim]) -> Vec<Vec<u32>> {
    let mut fab_map: Vec<Vec<u32>> = Vec::new();
    for _row in 0..1000 {
        let mut v = Vec::with_capacity(1000);
        for _c in 0..1000 {
            v.push(0)
        }
        fab_map.push(v);
    }

    for claim in claims {
        for r in claim.top..claim.top + claim.height {
            for c in claim.left..claim.left + claim.width {
                fab_map[r as usize][c as usize] += 1;
            }
        }
    }
    fab_map
}

fn count_overlapping(fab_map: &[Vec<u32>]) -> u32 {
    fab_map
        .iter()
        .map(|column| column.iter().filter(|val| **val > 1).count() as u32)
        .sum()
}

fn get_claims(filename: &str) -> Vec<Claim> {
    let f = std::fs::File::open(filename).unwrap();
    let r = BufReader::new(f);
    r.lines()
        .map(|line| line.unwrap().parse::<Claim>().unwrap())
        .collect()
}

#[derive(PartialEq, Debug)]
struct Claim {
    id: u32,
    width: u32,
    height: u32,
    left: u32,
    top: u32,
}


impl FromStr for Claim {
    type Err = Box<dyn Error>;

    fn from_str(claim: &str) -> Result<Self, Self::Err> {
        lazy_static! {
                static ref re: Regex = Regex::new(
                r"#(?P<id>\d*) @ (?P<left>\d*),(?P<top>\d*): (?P<width>\d*)x(?P<height>\d*)",
            )
            .expect("bad regex");
        }
        let caps = match re.captures(claim) {
            Some(caps) => caps,
            _ => bail!("failed to parse capture"),
        };

        Ok(Claim {
            left: caps
                .name("left")
                .expect("no left?")
                .as_str()
                .parse::<u32>()?,
            top: caps.name("top").expect("no top?").as_str().parse::<u32>()?,
            width: caps
                .name("width")
                .expect("no width?")
                .as_str()
                .parse::<u32>()?,
            height: caps
                .name("height")
                .expect("no height?")
                .as_str()
                .parse::<u32>()?,
            id: caps.name("id").expect("no id?").as_str().parse::<u32>()?,
        })
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    #[test]
    fn test_parse() {
        let hash = "#1 @ 1,3: 4x4".parse::<Claim>().unwrap();
        assert_eq!(
            hash,
            Claim {
                id: 1,
                left: 1,
                top: 3,
                width: 4,
                height: 4
            }
        );
    }

    #[test]
    fn test_input() {
        let claims = get_claims("test_input_a.txt");
        assert_eq!(count_overlapping(&map_claims(&claims)), 4);
    }
}
