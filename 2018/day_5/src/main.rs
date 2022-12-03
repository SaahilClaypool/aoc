use std::fs::File;
use std::io::prelude::*;
use std::time::Instant;

fn main() {
    let mut f = File::open("input.txt").expect("no such file");
    let mut content = String::new();
    f.read_to_string(&mut content)
        .expect("failed to read to string");
    let start = Instant::now();
    let result = react(&content);
    let end = Instant::now();
    let dur = end.duration_since(start);
    let millis = dur.as_secs() * 1000 + dur.subsec_millis() as u64;
    println!("Finished in : {} ms", millis);
    println!("answer is {}", result.len());

    let start = Instant::now();
    let min = min_reaction(&content);
    let end = Instant::now();
    let dur = end.duration_since(start);
    let millis = dur.as_secs() * 1000 + dur.subsec_millis() as u64;
    println!("Finished in : {} ms", millis);
    println!("answer is {}", min.len());
}

fn min_reaction(polymer: &str) -> String {
    let mut min_str = String::from(polymer);
    let mut min_len = polymer.len();
    for c1 in "abcdefghijklmnopqrstuvwxyz".chars() {
        let smaller_polymer: String = polymer
            .chars()
            .filter(|c2| !c2.eq_ignore_ascii_case(&c1))
            .collect();
        let res = react(&smaller_polymer);
        if res.len() < min_len {
            min_len = res.len();
            min_str = res;
        }
    }

    min_str
}

/**
 * Input: string like "abBA"
 * keep reducing adjacent if they are the same letter but differ by case
 */
fn react(polymer: &str) -> String {
    // right now, do 2d scan
    let mut polymer: Vec<char> = polymer.chars().collect();
    let mut has_changed: bool = true;
    let mut last_change = 0;
    'outer: while has_changed && polymer.len() > 0 {
        for idx in last_change..polymer.len() - 1 {
            let c1 = polymer[idx];
            let c2 = polymer[idx + 1];
            if c1 != c2 && c1.eq_ignore_ascii_case(&c2) {
                polymer.remove(idx + 1);
                polymer.remove(idx);
                if idx != 0 {
                    last_change = idx - 1;
                }
                continue 'outer;
            }
        }
        has_changed = false;
    }

    polymer.into_iter().collect()
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_basic() {
        let input = "abBA";
        let result = react(&input);

        assert_eq!("", result);
    }

    #[test]
    fn test_more() {
        let input = "dabAcCaCBAcCcaDA";
        let result = react(&input);

        assert_eq!("dabCBAcaDA", result);
    }

    #[test]
    fn test_min() {
        let input = "dabAcCaCBAcCcaDA";
        let result = min_reaction(&input);

        assert_eq!(4, result.len());
    }
}
