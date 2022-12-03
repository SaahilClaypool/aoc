use regex::Regex;
use std::collections::HashMap;
use std::fs::File;
use std::io::prelude::*;
use std::io::BufReader;
use std::iter;

fn main() {
    println!("Hello, world!");
    let input = parse_input("input.txt");
    let sorted = sorted_order(input);
    let s: String = sorted.iter().collect();
    println!("s: {}", s);
    let input = parse_input("input.txt");
    let t = sorted_multi(input, 60, 6);
    println!("with workers, takes : {}", t);
}

struct Dep {
    c: char,
    is_finished: bool,
}

fn parse_input(filename: &str) -> HashMap<char, Vec<char>> {
    // Regex::new(r"Guard #(?P<ID>\d*) begins shift").expect("failed regex");
    let re = Regex::new(r"Step (?P<Dep>.) must be finished before step (?P<Step>.) can begin.")
        .expect("failed regex");
    eprintln!("{:#?}", re);
    let f = File::open(filename).unwrap();
    let reader = BufReader::new(f);

    let mut depends_on: HashMap<char, Vec<char>> = HashMap::new();

    for line in reader.lines() {
        let line = line.unwrap();
        eprintln!("{}", line);
        let parsed_line = re.captures(&line).unwrap();
        let step = parsed_line
            .name("Step")
            .unwrap()
            .as_str()
            .chars()
            .nth(0)
            .unwrap();
        let dep = parsed_line
            .name("Dep")
            .unwrap()
            .as_str()
            .chars()
            .nth(0)
            .unwrap();
        depends_on.entry(step).or_default().push(dep);
        if !depends_on.contains_key(&dep) {
            depends_on.insert(dep, Vec::new());
        }
    }

    depends_on
}

fn count_unfinished(deps: &Vec<char>, finished: &Vec<char>) -> u32 {
    return deps.iter().filter(|d| !finished.contains(d)).count() as u32;
}

fn sorted_order(deps: HashMap<char, Vec<char>>) -> Vec<char> {
    let mut finished = vec![];
    loop {
        let mut ready = vec![];
        for (c, d) in deps.iter() {
            if finished.contains(c) {
                continue;
            }
            let count = count_unfinished(d, &finished);
            if count == 0 {
                ready.push(c);
            }
        }
        if ready.len() == 0 {
            break;
        }
        ready.sort();
        eprintln!("ready: {:?}", ready);
        finished.push(*ready[0]);
    }
    finished
}

fn sorted_multi(deps: HashMap<char, Vec<char>>, base_time: u32, num_workers: u32) -> u32 {
    let mut finished = vec![];
    let mut in_progress = vec![];
    let mut t = 0;
    loop {
        let mut finished_indexes = vec![];
        for (i, (progress_char, start_time)) in in_progress.iter().enumerate() {
            if t - *start_time as u32 == base_time + char_val(*progress_char) {
                finished.push(*progress_char);
                finished_indexes.push(i)
            }
        }
        finished_indexes.reverse();
        for i in finished_indexes {
            in_progress.remove(i);
        }
        let mut ready = vec![];
        let prog_char: Vec<char> = in_progress.iter().map(|(c, _)| *c).collect();
        for (c, d) in deps.iter() {
            if finished.contains(c) || prog_char.contains(c) {
                continue;
            }
            let count = count_unfinished(d, &finished);
            if count == 0 {
                ready.push(c);
            }
        }

        if ready.len() == 0 && in_progress.len() == 0{
            break;
        }
        ready.sort();

        for _worker in 0..num_workers - in_progress.len() as u32 {
            if ready.len() != 0 {
                in_progress.push((*ready.remove(0), t));
            }
        }
        eprintln!("{} progress: {:?} finished: {:?}", t, in_progress, finished);
        t += 1;
    }
    t
}

fn char_val(c: char) -> u32 {
    (c as u8 - 'A' as u8 + 1) as u32
}

#[cfg(test)]
mod tests {
    use super::*;
    #[test]
    fn test() {
        let input = parse_input("test.txt");
        let sorted = sorted_order(input);
        assert_eq!(sorted, vec!['C', 'A', 'B', 'D', 'F', 'E'])
    }

    #[test]
    fn test_multi() {
        let input = parse_input("test.txt");
        let sorted = sorted_multi(input, 0, 2);
        assert_eq!(sorted, 15)
    }
}
