use std::collections::HashMap;
use std::collections::HashSet;
fn main() {
    part_b();
    // let mut state = State::new(510, Pos { x: 10, y: 10 });
    // state.update_all();
    // let dist = state.path_find();
    // assert_eq!(dist, 45);
}
fn part_b() {
    let depth = 10914;
    let target = Pos { x: 9, y: 739 };
    let mut state = State::new(depth, target);
    state.update_all();
    let dist = state.path_find();
    println!("dist is {}", dist);
}

fn part_a() {
    let depth = 10914;
    let target = Pos { x: 9, y: 739 };
    let mut state = State::new(depth, target);
    state.update_all();
    let score = state.score_to_target();
    println!("score is {}", score);
}

#[derive(PartialEq, Eq, Hash, Copy, Clone, Debug)]
enum Gear {
    Torch,
    Climb,
    Neither,
}

const X_MUL: usize = 16807;
const Y_MUL: usize = 48271;
const MOD: usize = 20183;
const ROCKY: usize = 0;
const WET: usize = 1;
const NARROW: usize = 2;
const EXTRA: usize = 50;

struct State {
    depth: usize,
    current: Vec<Vec<Option<usize>>>,
    target: Pos,
}

impl State {
    fn elevel(&mut self, pos: &Pos) -> usize {
        let val = (pos.gidx(self) + self.depth) % MOD;
        self.current[pos.y][pos.x] = Some(val);
        return val;
    }
    fn new(depth: usize, target: Pos) -> State {
        State {
            depth,
            current: vec![vec![None; target.x + EXTRA + 1]; target.y + EXTRA + 1],
            target,
        }
    }
    fn update_all(&mut self) {
        for y in 0..=self.target.y + EXTRA {
            for x in 0..=self.target.x + EXTRA {
                let p = Pos { x, y };
                self.elevel(&p);
            }
        }
    }
    fn score_to_target(&self) -> usize {
        let mut s = 0;
        for y in 0..=self.target.y {
            for x in 0..=self.target.x {
                let n = match self.current[y][x].expect("update before scoring") % 3 {
                    ROCKY => 0,
                    WET => 1,
                    NARROW => 2,
                    _ => 0,
                };
                s += n;
            }
        }
        s
    }
    fn get_elevel(&self, pos: &Pos) -> usize {
        self.current[pos.y][pos.x].unwrap()
    }
    fn get_type(&self, pos: &Pos) -> usize {
        self.get_elevel(pos) % 3
    }
    fn possible_gear(&self, spot: &Pos) -> Vec<(Gear, Pos)> {
        let mut v = vec![];
        match self.get_type(&spot) {
            ROCKY => {
                v.push((Gear::Torch, *spot));
                v.push((Gear::Climb, *spot));
            }
            WET => {
                v.push((Gear::Neither, *spot));
                v.push((Gear::Climb, *spot));
            }
            NARROW => {
                v.push((Gear::Neither, *spot));
                v.push((Gear::Torch, *spot));
            }
            t => panic!("weird type: {}", t),
        }
        v
    }
    /// update before calling this
    fn path_find(&self) -> usize {
        // dist in minutes
        let mut dists: HashMap<(Gear, Pos), ((Gear, Pos), usize)> = HashMap::new();
        dists.insert(
            (Gear::Torch, Pos { x: 0, y: 0 }),
            ((Gear::Torch, Pos { x: 0, y: 0 }), 0),
        );
        let mut finished: HashSet<(Gear, Pos)> = HashSet::new();
        let mut active: Vec<(Gear, Pos)> = vec![(Gear::Torch, Pos { x: 0, y: 0 })];
        // build distances
        while !active.is_empty() {
            let (cur_gear, cur) = active.pop().unwrap();
            let surrounding = cur.surrounding(&self.target);
            let mut edges = vec![];
            for spot in surrounding {
                let mut possible = self.possible_gear(&spot);
                edges.append(&mut possible);
            }
            edges.append(&mut self.possible_gear(&cur));
            for (gear, s) in edges {
                let mut t = 0; 
                if gear != cur_gear {
                    if s == cur {
                        t = 7;
                    }
                    else {
                        continue;
                    }
                } 
                else {
                    t = 1;
                }
                // add the current distance
                t += dists[&(cur_gear, cur)].1;

                if t <= dists.entry((gear, s)).or_insert(((cur_gear, cur), t)).1 {
                    dists.insert((gear, s), ((cur_gear, cur), t));
                    finished.remove(&(gear, s));
                }
                if !finished.contains(&(gear, s)) && !active.contains(&(gear, s)) {
                    active.push((gear, s));
                }
            }
            active.sort_by(|(g, p), (g2, p2)| dists[&(*g2, *p2)].1.cmp(&dists[&(*g, *p)].1));
            finished.insert((cur_gear, cur));
        }

        // reconstruct path 
        let mut path = vec![];
        let ((mut from_gear, mut from_val), mut _val) = dists[&(Gear::Torch, self.target)];
        path.push(from_val);
        while (from_gear, from_val) != (Gear::Torch, Pos{x: 0, y: 0}) {
            // println!("from: {:?}", (from_gear, from_val));
            let ((gear, spot), _val) = dists[&(from_gear, from_val)];
            from_gear = gear;
            from_val = spot;
            path.push(from_val);
        }
        path.reverse();
        for spot in path {
            // println!("{:?}", spot);
        }

        // now lets path find
        return dists[&(Gear::Torch, self.target)].1;
    }
}

#[derive(PartialEq, Debug, Copy, Clone, Eq, Hash)]
struct Pos {
    x: usize,
    y: usize,
}

impl Pos {
    fn gidx(&self, state: &mut State) -> usize {
        if (self.x == 0 && self.y == 0) || (self.x == state.target.x && self.y == state.target.y) {
            0
        } else if self.y == 0 {
            self.x * X_MUL
        } else if self.x == 0 {
            self.y * Y_MUL
        } else {
            let a = match state.current[self.y][self.x - 1] {
                Some(val) => val,
                None => {
                    let a = Pos {
                        x: self.x - 1,
                        y: self.y,
                    };
                    let a = state.elevel(&a);
                    a
                }
            };
            let b = match state.current[self.y - 1][self.x] {
                Some(val) => val,
                None => {
                    let b = Pos {
                        x: self.x,
                        y: self.y - 1,
                    };
                    let b = state.elevel(&b);
                    b
                }
            };
            a * b
        }
    }

    fn surrounding(&self, target: &Pos) -> Vec<Self> {
        let mut v = vec![];
        // so we can handle switching gear
        if self.x > 0 {
            v.push(Pos {
                x: self.x - 1,
                y: self.y,
            });
        }
        if self.x < target.x + EXTRA {
            v.push(Pos {
                x: self.x + 1,
                y: self.y,
            });
        }
        if self.y > 0 {
            v.push(Pos {
                x: self.x,
                y: self.y - 1,
            });
        }
        if self.y < target.y + EXTRA {
            v.push(Pos {
                x: self.x,
                y: self.y + 1,
            });
        }
        v
    }
}

impl ToString for State {
    fn to_string(&self) -> String {
        let mut st = String::with_capacity(EXTRA * self.depth * self.depth);
        for (yi, row) in self.current.iter().enumerate() {
            for (xi, col) in row.iter().enumerate() {
                let c = match col {
                    Some(v) => match v % 3 {
                        ROCKY => '.',
                        WET => '=',
                        NARROW => '|',
                        _ => '?',
                    },
                    None => '?',
                };
                if yi == 0 && xi == 0 {
                    st.push('M');
                } else if xi == self.target.x && yi == self.target.y {
                    st.push('T');
                } else {
                    st.push(c);
                }
            }
            st.push('\n');
        }
        st
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    #[test]
    fn test() {
        let mut state = State::new(510, Pos { x: 10, y: 10 });
        assert_eq!(state.elevel(&Pos { x: 0, y: 0 }), 510);
        assert_eq!(state.elevel(&Pos { x: 1, y: 0 }), 17317);
        assert_eq!(state.elevel(&Pos { x: 0, y: 1 }), 8415);
        assert_eq!(state.elevel(&Pos { x: 1, y: 1 }), 1805);
    }
    #[test]
    fn test_example() {
        let mut state = State::new(510, Pos { x: 10, y: 10 });
        state.update_all();
        let s = state.score_to_target();
        assert_eq!(s, 114);
    }
    #[test]
    fn test_example_path() {
        let mut state = State::new(510, Pos { x: 10, y: 10 });
        state.update_all();
        let dist = state.path_find();
        assert_eq!(dist, 45);
    }
}
