use std::cmp::{max, min, Reverse};
use std::collections::BinaryHeap;
use std::str::FromStr;

fn main() {
    // let input = include_str!("../input.txt");
    // let state = State::from_str(input).unwrap();
    // let strongest = state.strongest();
    // println!("strongest has {} in range", strongest);
    // part 2
    // let input = include_str!("../sample2.txt");
    // let state = State::from_str(input).unwrap();
    // let best_pos = state.best_place();
    // let dist = best_pos.dist(&Pos { x: 0, y: 0, z: 0 });
    // println!("best place: {:?}", best_pos);
    // println!("dist to best place: {}", dist);
    // let best_pos = state.best_place_brute();
    // let dist = best_pos.dist(&Pos { x: 0, y: 0, z: 0 });
    // println!("best place: {:?}", best_pos);
    // println!("dist to best place: {}", dist);
    test_best_place_partition();
}
fn test_best_place_partition() {
    let input = include_str!("../input.txt");
    let state = State::from_str(input).unwrap();
    let best_pos = state.best_place();
    let dist = best_pos.dist(&Pos { x: 0, y: 0, z: 0 });
    println!("dist to {:?} is {}", best_pos, dist);
    let temp_bot = Bot {pos: best_pos, r: 0};
    let count = state.bots.iter().filter(|bot| bot.in_range(&temp_bot)).count();
    println!("temp bot in range of {}", count);
}

#[derive(Debug, PartialEq, Eq, Copy, Clone, Hash)]
struct Pos {
    x: i32,
    y: i32,
    z: i32,
}

fn test_point (pos: &Pos, bots: &[Bot]) -> usize {
    bots.iter().filter(|bot| {
        bot.in_range(&Bot {pos: *pos, r: 0}) 
    })
    .count()
}

impl Pos {
    /// manhatten distance
    fn dist(&self, other: &Self) -> usize {
        (self.x - other.x).abs() as usize
            + (self.y - other.y).abs() as usize
            + (self.z - other.z).abs() as usize
    }
}
impl FromStr for Pos {
    type Err = Box<dyn std::error::Error>;

    fn from_str(st: &str) -> Result<Self, Self::Err> {
        let st = String::from(&st[5..]);
        let st = st.replace(">", "");
        let mut parts = st.split(",");
        let x = parts.nth(0).unwrap().parse().unwrap();
        let y = parts.nth(0).unwrap().parse().unwrap();
        let z = parts.nth(0).unwrap().parse().unwrap();
        Ok(Pos { x, y, z })
    }
}

#[derive(Debug, PartialEq, Eq, Copy, Clone, Hash)]
struct Bot {
    pos: Pos,
    r: usize,
}

impl Bot {
    fn in_range(&self, other: &Self) -> bool {
        self.pos.dist(&other.pos) <= self.r
    }

    fn points_in_range(&self) -> Vec<Pos> {
        println!("points for {:?}", self);
        let r = self.r as i32;
        let mut points = vec![];
        for x in self.pos.x - r..self.pos.x + r {
            for y in self.pos.y - r..self.pos.y + r {
                for z in self.pos.z - r..self.pos.z + r {
                    let pos = Pos { x, y, z };
                    let temp_bot = Bot { r: 0, pos };
                    if self.in_range(&temp_bot) {
                        points.push(pos);
                    }
                }
            }
        }
        points
    }

    fn in_range_sq(&self, other: &Self) -> bool {
        let r = self.r as i32;
        let xmin = self.pos.x - r;
        let xmax = self.pos.x + r;
        let ymin = self.pos.y - r;
        let ymax = self.pos.y + r;
        let zmin = self.pos.z - r;
        let zmax = self.pos.z + r;
        other.pos.x >= xmin
            && other.pos.x <= xmax
            && other.pos.y >= ymin
            && other.pos.y <= ymax
            && other.pos.z >= zmin
            && other.pos.z <= zmax
    }
}

impl FromStr for Bot {
    type Err = Box<dyn std::error::Error>;

    fn from_str(st: &str) -> Result<Self, Self::Err> {
        let mut parts = st.split(", ");
        let pos = parts.nth(0).unwrap().parse().unwrap();
        let r: usize = parts.nth(0).unwrap()[2..].parse().unwrap();

        Ok(Bot { r, pos })
    }
}

#[derive(Debug, Clone)]
struct State {
    bots: Vec<Bot>,
}

impl State {
    /// count number of bots in range of the largest radius
    fn strongest(&self) -> usize {
        let max_bot = self.bots.iter().max_by(|a, b| a.r.cmp(&b.r)).unwrap();
        let count_in_range = self
            .bots
            .iter()
            .filter(|other| max_bot.in_range(other))
            .count();
        count_in_range
    }

    fn bounds(&self) -> (Pos, Pos) {
        let mut min_x = 0;
        let mut min_y = 0;
        let mut min_z = 0;
        let mut max_x = 0;
        let mut max_y = 0;
        let mut max_z = 0;

        for bot in &self.bots {
            let pos = &bot.pos;
            min_x = min(min_x, pos.x);
            min_y = min(min_y, pos.y);
            min_z = min(min_z, pos.z);
            max_x = max(max_x, pos.x);
            max_y = max(max_y, pos.y);
            max_z = max(max_z, pos.z);
        }

        (
            Pos {
                x: min_x,
                y: min_y,
                z: min_z,
            },
            Pos {
                x: max_x,
                y: max_y,
                z: max_z,
            },
        )
    }

    /// convert all to log space.
    /// Find the best. Then, search from there
    fn best_log_place(&self) -> Pos {
        let log_bots: Vec<Bot> = self
            .bots
            .iter()
            .map(|bot| Bot {
                pos: Pos {
                    x: (bot.pos.x.abs() as f32).log2() as i32 * bot.pos.x.signum(),
                    y: (bot.pos.y.abs() as f32).log2() as i32 * bot.pos.x.signum(),
                    z: (bot.pos.z.abs() as f32).log2() as i32 * bot.pos.x.signum(),
                },
                r: (bot.r as f32).log2() as usize,
            })
            .collect();

        let mut log_state = self.clone();
        log_state.bots = log_bots;
        log_state.best_place_brute()
    }

    fn best_place_brute(&self) -> Pos {
        let (min_pos, max_pos) = self.bounds();
        let mut best_pos = Pos { x: 0, y: 0, z: 0 };
        let mut best_count = 0;
        for x in min_pos.x..=max_pos.x {
            for y in min_pos.y..=max_pos.y {
                for z in min_pos.z..=max_pos.z {
                    let mut c = 0;
                    let temp_bot = Bot {
                        r: 0,
                        pos: Pos { x, y, z },
                    };
                    for bot in &self.bots {
                        if bot.in_range(&temp_bot) {
                            c += 1;
                        }
                    }
                    let temp_pos = Pos { x, y, z };
                    if c > best_count
                        || (c == best_count
                            && temp_pos.dist(&Pos { x: 0, y: 0, z: 0 })
                                < best_pos.dist(&Pos { x: 0, y: 0, z: 0 }))
                    {
                        best_count = c;
                        best_pos = Pos { x, y, z }
                    }
                }
            }
        }

        best_pos
    }

    /// recursive partition of space.
    fn best_place(&self) -> Pos {
        let bounds = self.bounds();
        println!("bounds: {:?}", bounds);
        let xd = (bounds.0.x - bounds.1.x).abs();
        let yd = (bounds.0.y - bounds.1.y).abs();
        let zd = (bounds.0.z - bounds.1.z).abs();
        let d = max(xd, max(yd, zd));
        // let d = xd + yd + zd;
        let center = Pos {
            x: bounds.0.x + d,
            y: bounds.0.y + d,
            z: bounds.0.z + d,
        };
        let mut spaces = BinaryHeap::new();
        spaces.push(Space {
            matches: self.bots.len(),
            pos: Bot {
                pos: center,
                r: (xd + yd + zd) as usize,
            },
        });
        while let Some(space) = spaces.pop() {
            let space = space;
            // println!("center is : {:?}", space);
            if space.pos.r == 0 {
                println!("best is : {:?}", space);
                return space.pos.pos;
            }
            let sub_spaces = space.sub_spaces();
            for mut sub_space in sub_spaces.into_iter() {
                let matches = self
                    .bots
                    .iter()
                    .filter(|bot| sub_space.in_range(&bot))
                    .count();
                // let matches = self.bots.iter().filter(|bot| bot.in_range(&sub_space.pos)).count();
                sub_space.matches = matches;
                spaces.push(sub_space);
                // println!("Spaces: {:?}", spaces);
            }
            // println!("spaces: {:#?}", spaces);
            // panic!("");
        }
        unimplemented!()
    }
}

#[derive(PartialEq, Eq, Debug)]
struct Space {
    matches: usize,
    pos: Bot,
}

impl Space {
    fn in_range(&self, bot: &Bot) -> bool {
        let d = bot.pos.dist(&self.pos.pos);
        let sum_rad = bot.r + self.pos.r * 3;
        let matches = d <= sum_rad;
        if matches {
            return true;
        }
        return false;
    }

    // fn in_range(&self, bot: &Bot) -> bool {
    //     return self.pos.in_range_sq(bot);
    // }

    fn sub_spaces(&self) -> Vec<Self> {
        let bot = self.pos;
        let d = bot.r / 2;
        // let r = 2 * (bot.r / 3);
        let r = d;
        // split the space into 8 little spaces.
        let mut spaces = vec![];
        for xi in 0..2 {
            for yi in 0..2 {
                for zi in 0..2 {
                    let center = Pos {
                        x: bot.pos.x - d as i32 + bot.r as i32 * xi,
                        y: bot.pos.y - d as i32 + bot.r as i32 * yi,
                        z: bot.pos.z - d as i32 + bot.r as i32 * zi,
                    };
                    // println!("from {:?} created corner {:?}", bot, center);
                    let new_space = Space {
                        pos: Bot { pos: center, r },
                        matches: 0,
                    };
                    spaces.push(new_space);
                }
            }
        }
        spaces
    }

    fn score(&self) -> i64 {
        // larger radius slightly worse
        let rad_score = -1 * self.pos.r as i64;
        // mostly just care about the number of matches
        let match_score = (self.matches * 5000000) as i64;
        // let dist_score = 0;
        // let dist_score =
        //     -1 * (self.pos.pos.dist(&Pos { x: 0, y: 0, z: 0 }) as f64 / 100000000f64) as i64;
        let dist_score =
            -1 * (self.pos.pos.dist(&Pos { x: 0, y: 0, z: 0 }) as f64 / 1f64) as i64;
        rad_score + match_score + dist_score
    }
}

impl PartialOrd for Space {
    fn partial_cmp(&self, other: &Self) -> Option<std::cmp::Ordering> {
        let v1 = self.score();
        let v2 = other.score();
        Some(v1.cmp(&v2))
    }
}

impl Ord for Space {
    fn cmp(&self, other: &Self) -> std::cmp::Ordering {
        self.partial_cmp(&other).unwrap()
    }
}

impl FromStr for State {
    type Err = Box<dyn std::error::Error>;

    fn from_str(st: &str) -> Result<Self, Self::Err> {
        let mut v = vec![];
        for line in st.lines() {
            v.push(Bot::from_str(line)?);
        }
        Ok(State { bots: v })
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    #[test]
    fn test() {
        let input = include_str!("../sample.txt");
        let state = State::from_str(input).unwrap();
        eprintln!("state: {:#?}", state);
        assert_eq!(0, 0);
    }
    #[test]
    fn test_in_range() {
        let input = include_str!("../sample.txt");
        let state = State::from_str(input).unwrap();
        assert_eq!(state.strongest(), 7);
    }
    #[test]
    fn test_best_place() {
        let input = include_str!("../sample2.txt");
        let state = State::from_str(input).unwrap();
        let best_pos = state.best_place_brute();
        let dist = best_pos.dist(&Pos { x: 0, y: 0, z: 0 });
        assert_eq!(dist, 36);
    }
    #[test]
    fn test_best_place_partition() {
        let input = include_str!("../sample2.txt");
        let state = State::from_str(input).unwrap();
        let best_pos = state.best_place();
        let dist = best_pos.dist(&Pos { x: 0, y: 0, z: 0 });
        assert_eq!(dist, 36);
    }

}
