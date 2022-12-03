use std::fs::File;
use std::io::prelude::*;
use std::time::Instant;

fn main() {
    let start = Instant::now();
    let input = read_input("input.txt");
    let res = input.count_meta();
    let end = Instant::now();
    println!("Finished in : {} ms", end.duration_since(start).subsec_millis());
    println!("result is {}", res);

    // part 2

    let start = Instant::now();
    let res = input.value();
    let end = Instant::now();
    println!("Finished in : {} ms", end.duration_since(start).subsec_millis());
    println!("result is {}", res);
}

#[derive(Debug)]
struct Node {
    num_children: u32,
    children: Vec<Node>,
    num_meta: u32,
    meta: Vec<u32>,
}

impl Node {
    fn from(input: &mut Vec<&str>) -> Self {
        let mut me: Node = Node {
            num_children: 0,
            num_meta: 0,
            children: Vec::new(),
            meta: Vec::new(),
        };
        me.num_children = input.remove(0).parse::<u32>().unwrap();
        me.num_meta = input.remove(0).parse::<u32>().unwrap();

        for _child_num in 0..me.num_children {
            me.children.push(Node::from(input));
        }

        for _meta in 0..me.num_meta {
            me.meta.push(input.remove(0).parse::<u32>().unwrap())
        }

        me
    }

    fn value(&self) -> u32 {
        if self.num_children == 0 {
            let sum_meta: u32 = self.meta.iter().sum();
            return sum_meta;
        } else {
            let mut child_sum = 0;
            for meta in &self.meta {
                if *meta <= self.num_children && *meta > 0 {
                    let meta = (*meta - 1) as usize;
                    child_sum += self.children[meta].value()
                }
            }
            return child_sum;
        }
    }

    fn count_meta(&self) -> u32 {
        let children_sum: u32 = self.children.iter().map(|child| child.count_meta()).sum();
        let total_sum: u32 = children_sum ;
        let my_sum: u32 = self.meta.iter().sum();
        total_sum + my_sum
    }
}

fn read_input(filename: &str) -> Node {
    let mut file = File::open(filename).unwrap();
    let mut content = String::new();
    file.read_to_string(&mut content).expect("didn't open file");
    let mut parts: Vec<&str> = content.split(" ").collect();

    Node::from(&mut parts)
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test() {
        let input = read_input("test.txt");
        eprintln!("{:#?}: ", input);
        assert_eq!(input.count_meta(), 138);
    }

    #[test]
    fn test2() {
        let input = read_input("test.txt");
        eprintln!("{:#?}: ", input);
        assert_eq!(input.children[0].value(), 33);
        assert_eq!(input.children[1].children[0].value(), 99);
        assert_eq!(input.value(), 66);
    }
}
