use std::fs::File;
use std::io::prelude::*;
use std::collections::HashMap;

fn main() {
    let hash = compute_hash("input.txt");
    println!("Hash is : {}", hash);

    let matching = closest_match("input.txt");
    println!("matching: {}", matching);
}

fn compute_hash(filename: &str) -> i32 {
    let mut f = File::open(&filename).expect("failed to open file");
    let mut contents = String::new();
    f.read_to_string(&mut contents).expect("failed to read to string");
    let mut doubles = 0;
    let mut triples = 0;


    for line in contents.lines() {
        let mut char_map = HashMap::<char, u32>::new();
        for c in line.chars() {
            *char_map.entry(c).or_insert(0) += 1;
        }

        if char_map.iter().any(|(_, v)| *v == 2) {
            doubles += 1;
        }
        if char_map.iter().any(|(_, v)| *v == 3) {
            triples += 1;
        }
    }   
    let hash = doubles * triples;
    println!("doubles: {} triples: {} hash: {}", doubles, triples, hash);
    hash
}

fn closest_match(filename: &str) -> String {
    let mut f = File::open(&filename).expect("failed to open file");
    let mut contents = String::new();
    f.read_to_string(&mut contents).expect("failed to read to string");

    let all_entries: Vec<&str> = contents.lines().collect();
    for entry in &all_entries {
        'inner: for other in &all_entries {
            if entry == other {
                continue 'inner;
            }
            let mut matching_chars = String::new();
            for (char1, char2) in entry.chars().zip(other.chars()) {
                if char1 == char2 {
                    matching_chars.push(char1);
                }
            }
            if matching_chars.len() == other.len() - 1 {
                return matching_chars;
            }
        }
    }
    String::new()
}

#[cfg(test)]
mod tests {
    use super::{compute_hash, closest_match};
    #[test]
    fn test_input() {
        let hash = compute_hash("test.txt");
        assert_eq!(hash, 12);
    }

    #[test]
    fn test_match() {
        let matching = closest_match("testb.txt");
        eprintln!("matching: {}", matching);
        let does_match = matching.chars().all(|c| "fgij".contains(c)) && matching.len() == "fgij".len();
        assert!(does_match);
    }
}
