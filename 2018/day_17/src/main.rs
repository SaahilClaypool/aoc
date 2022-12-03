use std::collections::HashMap;
use std::str::FromStr;
fn main() {
    let input_st = include_str!("dad.txt");
    let mut input: Map = input_st.parse().unwrap();
    // println!("Map:\n{}", input.to_string());
    input.fill(&input.spring.clone());
    println!("Map:\n{}", input.to_string());
    let og_input: Map = input_st.parse().unwrap();
    for (l, t) in og_input.squares.iter() {
        match input.squares.get(l) {
            None => panic!("How did I lose a spot"),
            Some(new_t) => if new_t != t {
                panic!("Jeez, changed {:?} from {} to {}", l, char::from(t), char::from(new_t));
            }
        }
    }
    println!("water reaches: {}", input.count());
    println!("water holds: {}", input.count_water());
}

#[derive(Hash, Eq, PartialEq, Debug, Copy, Clone)]
struct Loc {
    x: usize,
    y: usize,
}

#[derive(PartialEq, Debug)]
struct Map {
    squares: HashMap<Loc, Type>,
    spring: Loc,
    min: Loc,
    max: Loc,
}

impl FromStr for Map {
    type Err = Box<dyn std::error::Error>;

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        let mut squares = HashMap::new();
        let mut min_x = 1000;
        let mut min_y = 1000;
        let mut max_x = 0;
        let mut max_y = 0;
        for line in s.lines() {
            let xy: Vec<&str> = line.split(", ").collect();
            let val = xy[0][2..].parse::<usize>().unwrap();
            let range_slice: Vec<&str> = xy[1][2..].split("..").collect();
            let r1: usize = range_slice[0].parse().unwrap();
            let r2: usize = range_slice[1].parse().unwrap();
            let (x1, x2, y1, y2) = match xy[0].chars().nth(0).unwrap() {
                'x' => (val, val, r1, r2),
                _ => (r1, r2, val, val),
            };
            if x1 < min_x {
                min_x = x1;
            }
            if x2 > max_x {
                max_x = x2;
            }
            if y1 < min_y {
                min_y = y1;
            }
            if y2 > max_y {
                max_y = y2;
            }
            for x in x1..=x2 {
                for y in y1..=y2 {
                    let l = Type::clay;
                    squares.insert(Loc { x, y }, l);
                }
            }
        }

        Ok(Self {
            squares,
            spring: Loc { x: 500, y: 0 },
            min: Loc {
                x: min_x - 2,
                y: min_y,
            },
            max: Loc {
                x: max_x + 2,
                y: max_y,
            },
        })
    }
}

impl Map {
    fn is_dry(&self, loc: &Loc) -> bool {
        match self.squares.get(loc) {
            Some(t) => match t {
                Type::dry => true,
                _ => false,
            },
            None => false,
        }
    }
    fn is_clay(&self, loc: &Loc) -> bool {
        match self.squares.get(loc) {
            Some(t) => match t {
                Type::clay => true,
                _ => false,
            },
            None => false,
        }
    }
    fn is_water(&self, loc: &Loc) -> bool {
        match self.squares.get(loc) {
            Some(t) => match t {
                Type::water => true,
                _ => false,
            },
            None => false,
        }
    }
    /// from the start point, fill the first container hit. return list of new spouts
    fn fill(&mut self, start: &Loc) -> Option<usize> {
        // find deepest clay
        let mut deepest = *start;
        eprintln!("start.y: {} max.y {}", start.y, self.max.y);
        for y in start.y + 1..=self.max.y {
            let loc = Loc { x: start.x, y };
            match self.squares.get(&loc) {
                Some(t) => match t {
                    Type::clay | Type::water => {
                        deepest = loc;
                        deepest.y -= 1;
                        break;
                    }
                    _ => {}
                },
                _ => {}
            }
            if !self.squares.contains_key(&loc) {
                self.squares.insert(loc, Type::dry);
            }
        }
        eprintln!("deepest is {:?}", deepest);
        if deepest.y == start.y {
            return None;
        }

        let mut y = deepest.y;
        while y > 0 {
            let loc = Loc { x: start.x, y };
            let row = self.get_row(&loc);
            let mut row = match row {
                Some(row) => row,
                None => break,
            };
            for water in row {
                self.squares.insert(water, Type::water);
            }
            y -= 1;
        }

        let spill_loc = Loc { x: start.x, y };
        let spill = self.get_out_flow(&spill_loc);
        eprintln!("spill from {:?} is {:?}", spill_loc, spill);
        for s in &spill {
            self.squares.insert(*s, Type::dry);
        }
        match self.squares.get(&spill_loc) {
            Some(_t) => {}
            None => {
                self.squares.insert(spill_loc, Type::dry);
            }
        };
        if spill.len() > 0 {
            let mut left = spill[0];
            let mut below = left;
            left.x -= 1;
            below.y += 1;
            if !self.is_dry(&below) && !self.squares.contains_key(&left) {
                self.squares.insert(left.clone(), Type::dry);
                self.fill(&left);
            }
            let mut right = spill[spill.len() - 1];
            let mut below = right;
            right.x += 1;
            below.y += 1;
            if !self.is_dry(&below) && !self.squares.contains_key(&right) {
                self.squares.insert(right.clone(), Type::dry);
                self.fill(&right);
            }
        }

        None
    }

    /// given a point above a lake, find the left and right drop off points
    fn get_out_flow(&self, l: &Loc) -> Vec<Loc> {
        let mut left = *l;
        left.x -= 1;
        let mut spots = vec![*l];
        while left.x > self.min.x {
            if self.is_clay(&left) || self.is_water(&left) {
                break;
            }
            let mut below = left;
            below.y += 1;
            if !self.squares.contains_key(&below) {
                break;
            }
            spots.push(left);
            left.x -= 1;
        }
        spots.reverse();
        let mut right = *l;
        right.x += 1;
        while right.x < self.max.x {
            if self.is_clay(&right) || self.is_water(&right) {
                break;
            }
            let mut below = right;
            below.y += 1;
            if !self.squares.contains_key(&below) {
                break;
            }
            spots.push(right);
            right.x += 1;
        }
        spots
    }

    // return possibility of filling the row
    fn get_row(&self, l: &Loc) -> Option<Vec<Loc>> {
        let mut left = vec![];
        let mut right = vec![];
        let mut x = l.x;
        let y = l.y;

        while x >= self.min.x {
            let mut below = Loc { x, y };
            below.y += 1;
            if x == self.min.x {
                return None;
            }
            match self.squares.get(&below) {
                Some(t) => match t {
                    Type::dry => {
                        return None;
                    }
                    _ => {}
                },
                None => {
                    return None;
                }
            }
            let loc = Loc { x, y };

            match self.squares.get(&loc) {
                Some(t) => match t {
                    Type::clay => {
                        break;
                    }
                    _ => {}
                },
                None => {}
            }
            left.push(loc);
            x -= 1;
        }

        let mut x = l.x + 1;

        while x <= self.max.x {
            let mut below = Loc { x, y };
            below.y += 1;
            if x == self.max.x {
                return None;
            }
            match self.squares.get(&below) {
                Some(t) => match t {
                    Type::dry => {
                        return None;
                    }
                    _ => {}
                },
                None => {
                    return None;
                }
            }
            let loc = Loc { x, y };

            match self.squares.get(&loc) {
                Some(t) => match t {
                    Type::clay => {
                        break;
                    }
                    _ => {}
                },
                None => {}
            }
            right.push(loc);
            x += 1;
        }
        left.append(&mut right);
        Some(left)
    }

    fn count(&self) -> usize {
        return self
            .squares
            .iter()
            .filter(|(k, t)| match t {
                Type::dry | Type::water => {
                    k.y >= self.min.y
                },
                _ => false,
            })
            .count();
    }
    fn count_water(&self) -> usize {
        return self
            .squares
            .iter()
            .filter(|(k, t)| match t {
                Type::water => {
                    k.y >= self.min.y
                },
                _ => false,
            })
            .count();
    }
}

impl ToString for Map {
    fn to_string(&self) -> String {
        let spots = (self.max.x - self.min.x) * (self.max.y - self.min.y);
        let mut st = String::with_capacity(spots * 2);
        for y in self.min.y..=self.max.y {
            for x in self.min.x..=self.max.x {
                let c = match self.squares.get(&Loc { x, y }) {
                    Some(t) => char::from(t),
                    None => ' ',
                };
                st.push(c);
            }
            st.push('\n');
        }
        st
    }
}

#[derive(PartialEq, Debug)]
enum Type {
    water,
    clay,
    dry,
    spring,
}

impl From<char> for Type {
    fn from(c: char) -> Self {
        match c {
            '+' => Type::spring,
            '#' => Type::clay,
            '~' => Type::water,
            '|' => Type::dry,
            _ => panic!("bad character: {}", c),
        }
    }
}

impl From<&Type> for char {
    fn from(c: &Type) -> Self {
        match c {
            Type::spring => '+',
            Type::clay => '#',
            Type::water => '~',
            Type::dry => '|',
        }
    }
}
#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_parse() {
        let input = include_str!("test.txt");
        let input: Map = input.parse().unwrap();
        eprintln!("{:#?}", input);
    }
    #[test]
    fn test_row() {
        let input = include_str!("test.txt");
        let input: Map = input.parse().unwrap();
        eprintln!("{:#?}", input);
        let row = input.get_row(&Loc { x: 0, y: 0 });
        assert_eq!(row, None);
        let row = input.get_row(&Loc { x: 500, y: 6 });
        eprintln!("row: {:#?}", row);
        assert_eq!(row.unwrap().len(), 5);
    }

    #[test]
    fn test_test() {
        let input = include_str!("test.txt");
        let mut input: Map = input.parse().unwrap();
        input.fill(&input.spring.clone());
        assert_eq!(input.count(), 57);
    }
}
