/**
 * The list of fixed points in spacetime (your puzzle input) is a set of
 * four-dimensional coordinates. To align your device, acquire the hot
 * chocolate, and save the reindeer, you just need to find the number of
 * constellations of points in the list.
 *
 * Constellation: two pionts not more than 3 points apart
 *
 *  This is similar to a [DBSCAN](https://en.wikipedia.org/wiki/DBSCAN) algorithm I think
 *
 */
use std::collections::{HashMap, HashSet};

const MAX_DIST: i32 = 3;

fn main() {
    let st = include_str!("../input.txt");
    let state: State = st.parse().unwrap();
    let num_groups = state.find_groups();
    println!("there are {} constellations", num_groups);
}

struct State {
    stars: Vec<Pos>,
}

fn is_group_close(a: &HashSet<usize>, b: &HashSet<usize>, stars: &[Pos]) -> bool {
    for i in a.iter() {
        for j in b.iter() {
            if stars[*i].dist(&stars[*j]) <= MAX_DIST {
                return true;
            }
        }
    }
    return false;
}

impl State {
    /// find the number of gruops in the current number of stars.
    /// This is similar to the dbscan algorithm
    fn find_groups(&self) -> usize {
        // this keeps the indices of the points per group
        let mut groups: Vec<HashSet<usize>> = vec![];
        for (idx, _star) in self.stars.iter().enumerate() {
            groups.push({
                let mut set = HashSet::new();
                set.insert(idx);
                set
            });
        }

        loop {
            let mut has_changed = false;
            // joins is a list of pairwise constellations to join
            let mut joins: Vec<(usize, usize)> = vec![];
            for (i, group) in groups.iter().enumerate() {
                let mut j = i + 1;
                for other in &groups[j..] {
                    if is_group_close(group, other, &self.stars) {
                        has_changed = true;
                        joins.push((i, j));
                    }
                    j += 1;
                }
            }

            let mut subst: HashMap<usize, usize> = HashMap::new();
            // we know the right side of each pair will never be paired again
            joins.sort_by(|a, b| {
                // sort by the second value so that we always remove the biggest values first
                a.1.cmp(&b.1)
            });
            for (a, b) in joins.iter().rev() {
                let mut a = *a;
                let mut b = *b;
                // resolve a
                while subst.contains_key(&a) {
                    a = subst[&a];
                }
                while subst.contains_key(&b) {
                    b = subst[&b];
                }
                if a == b {
                    continue;
                }
                for b_val in groups[b].clone().iter() {
                    groups[a].insert(*b_val);
                }
                subst.insert(b, a);
            }
            let mut keys: Vec<&usize> = subst.keys().collect();
            keys.sort();
            for removed_index in keys.iter().rev() {
                groups.remove(**removed_index);
            }

            if !has_changed {
                return groups.len();
            }
        }
    }
}

impl std::str::FromStr for State {
    type Err = Box<dyn std::error::Error>;

    /// input looks like 4 ordered pairs
    fn from_str(s: &str) -> Result<Self, Self::Err> {
        let stars = s.lines().map(|line| line.parse().unwrap()).collect();
        Ok(State { stars })
    }
}

#[derive(Copy, Clone, Debug, Hash)]
struct Pos(i32, i32, i32, i32);

impl Pos {
    fn dist(&self, other: &Self) -> i32 {
        (self.0 - other.0).abs()
            + (self.1 - other.1).abs()
            + (self.2 - other.2).abs()
            + (self.3 - other.3).abs()
    }
}

impl std::str::FromStr for Pos {
    type Err = Box<dyn std::error::Error>;

    /// input looks like 4 ordered pairs
    fn from_str(s: &str) -> Result<Self, Self::Err> {
        println!("st is {}", s);
        let mut parts = s.split(",");
        Ok(Pos(
            parts.next().unwrap().trim().parse().unwrap(),
            parts.next().unwrap().trim().parse().unwrap(),
            parts.next().unwrap().trim().parse().unwrap(),
            parts.next().unwrap().trim().parse().unwrap(),
        ))
    }
}
#[cfg(test)]
mod tests {
    use super::*;
    #[test]
    fn test_parse() {
        let st = include_str!("../test.txt");
        let state: State = st.parse().unwrap();
    }

    #[test]
    fn test_groups() {
        let st = include_str!("../test.txt");
        let state: State = st.parse().unwrap();
        let num_groups = state.find_groups();
        assert_eq!(num_groups, 2);
        let st = include_str!("../test2.txt");
        let state: State = st.parse().unwrap();
        let num_groups = state.find_groups();
        assert_eq!(num_groups, 4);
        let st = include_str!("../test3.txt");
        let state: State = st.parse().unwrap();
        let num_groups = state.find_groups();
        assert_eq!(num_groups, 3);
        let st = include_str!("../test4.txt");
        let state: State = st.parse().unwrap();
        let num_groups = state.find_groups();
        assert_eq!(num_groups, 8);
    }
}
