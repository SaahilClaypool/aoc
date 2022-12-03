use std::cell::RefCell;
use std::collections::HashMap;
use std::str::FromStr;
use std::string::ToString;

// 195888
fn main() {
    // let elf = &game.units[0];
    // let new_pos = elf.do_move(&game); // need to split up unit structure from game structure
    let input = include_str!("input.txt");
    part_b(input);
}

fn part_a() {
    let input = include_str!("input.txt");
    let mut game: Game = input.parse().unwrap();
    println!("{}", game.to_string());
    for i in 0..100 {
        game.do_round();
        for unit in &game.units {
            println!(
                "Unit {} {} health {}",
                unit.idx,
                unit.unit_type,
                &unit.hp.borrow()
            );
        }
        if game.winner() {
            println!("{}", game.to_string());
            println!("End after {} rounds", i);
            println!("score is {}", game.score());
            eprintln!("score is {}", game.score());
            break;
        }
        println!("{}\n{}\n---------------------", i, game.to_string());
    }
}

fn part_b(input: &str) -> i32 {
    let mut elf_damage = 3;
    let mut winning_score = 0;
    'outer: loop {
        let mut game: Game = input.parse().unwrap();
        game.elf_damage = elf_damage;
        println!("{}", game.to_string());
        for i in 0..1000 {
            game.do_round();
            for unit in &game.units {
                println!(
                    "Unit {} {} health {}",
                    unit.idx,
                    unit.unit_type,
                    &unit.hp.borrow()
                );
            }
            if game.winner() {
                if game.elf_winner() {
                    println!("{}", game.to_string());
                    println!("End after {} rounds", i);
                    println!("score is {}", game.score());
                    eprintln!("score is {}", game.score());
                    winning_score = game.score();
                    break 'outer;
                } else {
                    elf_damage += 1;
                    println!("goblin won at damage {}", elf_damage);
                    eprintln!("goblin won at damage {}", elf_damage);
                    continue 'outer;
                }
            }
            println!(
                "{}\n{}\n{}---------------------",
                i,
                game.to_string(),
                elf_damage
            );
        }
    }
    return winning_score;
}

enum Space {
    Unit(usize), // index of the unit that is there
    Wall,
    Empty,
}

#[derive(Hash, Eq, PartialEq, Debug, Clone, Copy)]
struct Pos {
    row: i32,
    col: i32,
}

impl Pos {
    fn dist(&self, other: &Self) -> usize {
        ((self.row as i32 - other.row as i32).abs() + (self.col as i32 - other.col as i32).abs())
            as usize
    }
}

const ELF: char = 'E';
const GOBLIN: char = 'G';
const NONE: char = 'N';

#[derive(Debug, Clone)]
struct Unit {
    unit_type: char,
    pos: RefCell<Pos>,
    idx: usize,
    hp: RefCell<i32>,
}

struct Game {
    units: Vec<Unit>,
    map: Vec<Vec<Space>>,
    round: usize,
    rows: usize,
    cols: usize,
    elf_damage: i32,
}

impl Unit {
    fn targets(&self, game: &Game) -> Vec<Pos> {
        let mut positions = vec![];
        for (r_idx, row) in game.map.iter().enumerate() {
            for (c_idx, col) in row.iter().enumerate() {
                if self.pos.borrow().col == c_idx as i32 && self.pos.borrow().row == r_idx as i32 {
                    continue;
                }
                match col {
                    Space::Unit(idx) => {
                        if self.idx == *idx {
                            continue;
                        }
                        if game.units[*idx].unit_type != self.unit_type
                            && *game.units[*idx].hp.borrow() > 0
                        {
                            positions.push(Pos {
                                row: r_idx as i32,
                                col: c_idx as i32,
                            });
                        }
                    }
                    _ => {}
                }
            }
        }
        positions
    }

    fn other(&self) -> char {
        match self.unit_type {
            ELF => GOBLIN,
            GOBLIN => ELF,
            _ => NONE,
        }
    }

    fn do_move(&self, game: &Game) -> Option<Pos> {
        let targets = self.targets(game);
        let adjacent: Vec<&Pos> = targets
            .iter()
            .filter(|other| other.dist(&self.pos.borrow()) == 1)
            .collect();
        if adjacent.len() != 0 {
            // println!("pos: {:?} {} is engaged to: {:?}", *self.pos.borrow(), self.unit_type, adjacent);
            return None;
        }
        let open: Vec<Vec<Pos>> = targets
            .iter()
            .map(|target| game.surrounding(target, NONE))
            .collect();
        let mut open_flat = vec![];
        for target in open.into_iter() {
            for space in target.into_iter() {
                open_flat.push(space);
            }
        }
        let dists = game.dists(&self.pos.borrow());
        let open_flat: Vec<Pos> = open_flat
            .into_iter()
            .filter(|pos| dists.contains_key(pos))
            .collect();
        // no path to anyone
        if open_flat.is_empty() {
            // println!("No Positions from {:?}", self);
            return None;
        }

        let mut min_dist = game.rows * game.cols * 2;
        let mut best_target = open_flat[0];
        for target in open_flat {
            let d_target = dists[&target];
            if d_target.dist < min_dist
                || d_target.dist == min_dist && game.order(&target) < game.order(&best_target)
            {
                min_dist = d_target.dist;
                best_target = target;
            }
        }

        let surrounding = game.surrounding(&self.pos.borrow(), NONE);
        let mut best_surrounding_move = 10000;
        let mut best_surrounding = *self.pos.borrow();
        for next in surrounding {
            let dists = game.dists(&next);
            if !dists.contains_key(&best_target) {
                // panic!("from {:?} missing key {:?}", next, best_target);
                continue;
            }
            let dist_to_target = dists[&best_target].dist;
            if dist_to_target < best_surrounding_move
                || dist_to_target == best_surrounding_move
                    && game.order(&next) < game.order(&best_surrounding)
            {
                best_surrounding_move = dist_to_target;
                best_surrounding = next;
            }
        }

        return Some(best_surrounding);
    }

    fn do_attack(&self, game: &Game) {
        let surrounding = game.surrounding(&self.pos.borrow(), self.other());
        let mut min_target: Option<usize> = None;
        // find lowest health surrounding enemy
        for next in surrounding {
            match game.map[next.row as usize][next.col as usize] {
                Space::Unit(uid) => {
                    let cur_target = &game.units[uid];
                    let cur_hp = *cur_target.hp.borrow();
                    match min_target {
                        Some(min_uid) => {
                            let target = &game.units[min_uid];
                            let min_hp = *target.hp.borrow();
                            if cur_hp < min_hp
                                || cur_hp == min_hp
                                    && game.order(&cur_target.pos.borrow())
                                        < game.order(&target.pos.borrow())
                            {
                                min_target = Some(uid);
                            }
                        }
                        None => {
                            min_target = Some(uid);
                        }
                    }
                }
                _ => {}
            }
        }

        match min_target {
            Some(target_id) => {
                game.units[target_id].damage(game.elf_damage);
            }
            None => {
                return;
            }
        }
    }

    fn damage(&self, elf_damage: i32) {
        let damage = match self.unit_type {
            GOBLIN => elf_damage, // goblins take X damage
            _ => 3,               // elves take 3 damage
        };
        let hp = *self.hp.borrow();
        let mut target_hp = self.hp.borrow_mut();
        *target_hp = hp - damage;
    }

    fn do_turn(&self, game: &Game) {
        if *self.hp.borrow() < 1 {
            return;
        }
        // println!("Doing turn for {} at {:?}", self.idx, *self.pos.borrow());
        let new_pos = self.do_move(game);
        match new_pos {
            Some(new_pos) => *self.pos.borrow_mut() = new_pos,
            _ => {}
        }
        self.do_attack(game);
    }
}

#[derive(Clone, Copy, Debug)]
struct DijkstraNode {
    visited: bool,
    dist: usize,
    pos: Pos,
    from: Option<Pos>,
}

impl Game {
    fn score(&self) -> i32 {
        let mut s = 0;
        for unit in &self.units {
            let hp = *unit.hp.borrow();
            if hp > 0 {
                s += hp;
            }
        }
        eprintln!("rounds: {} sum {}", self.round, s);
        return s as i32 * self.round as i32;
    }

    fn elf_winner(&self) -> bool {
        for unit in &self.units {
            if *unit.hp.borrow() < 1 && unit.unit_type == ELF {
                return false;
            }
        }
        return true;
    }
    fn winner(&self) -> bool {
        let mut has_elf = false;
        let mut has_gob = false;
        for r in 0..self.rows {
            for c in 0..self.cols {
                match self.map[r][c] {
                    Space::Unit(uid) => {
                        if self.units[uid].unit_type == GOBLIN {
                            has_gob = true;
                        } else if self.units[uid].unit_type == ELF {
                            has_elf = true;
                        }
                    }
                    _ => {}
                }
            }
        }
        let winner = !(has_gob && has_elf);
        winner
    }
    fn do_round(&mut self) {
        let mut units: Vec<usize> = self.units.iter().map(|unit| unit.idx).collect();
        units.sort_by(|a, b| {
            self.order(&self.units[*a].pos.borrow())
                .cmp(&self.order(&self.units[*b].pos.borrow()))
        });
        for uid in units {
            if *self.units[uid].hp.borrow() > 0 &&
                self.winner() {
                return;
            }
            let unit = &self.units[uid];
            unit.do_turn(&self);
            self.update_units();
        }
        self.round += 1;
        eprintln!("did round: {}", self.round);
    }

    fn update_units(&mut self) {
        // clear units
        for r in 0..self.rows {
            for c in 0..self.cols {
                match self.map[r][c] {
                    Space::Unit(_) => self.map[r][c] = Space::Empty,
                    _ => {}
                }
            }
        }
            // replace units
        for (uid, unit) in self.units.iter().enumerate() {
            let hp = *unit.hp.borrow();
            if hp > 0 {
                self.map[unit.pos.borrow().row as usize][unit.pos.borrow().col as usize] =
                    Space::Unit(uid);
            }
        }
    }

    fn order(&self, pos: &Pos) -> i32 {
        return pos.row * self.cols as i32 + pos.col;
    }

    /// get the number of steps to each position
    fn dists(&self, start: &Pos) -> HashMap<Pos, DijkstraNode> {
        let mut dists: HashMap<Pos, DijkstraNode> = HashMap::new();
        dists.insert(
            *start,
            DijkstraNode {
                visited: false,
                dist: 0,
                from: None,
                pos: *start,
            },
        );
        let mut unvisited: Vec<Pos> = vec![];
        unvisited.push(*start);
        let mut visited = 0;
        loop {
            if unvisited.len() == 0 {
                break;
            }
            let current = unvisited.pop().unwrap();

            let surrounding = self.surrounding(&current, NONE);
            // println!("{:?} surrounding: {:?}", current, surrounding);
            for next in surrounding.iter() {
                let current_distance = dists[&current].dist + 1;
                let mut value = dists.entry(*next).or_insert(DijkstraNode {
                    visited: false,
                    dist: current_distance,
                    from: Some(current),
                    pos: *next,
                });
                if value.dist > current_distance
                    || value.dist == current_distance && self.order(&current) < self.order(next)
                {
                    value.dist = current_distance;
                    value.from = Some(current);
                }
                if !value.visited && !unvisited.contains(next) {
                    unvisited.push(*next);
                }
            }
            dists.get_mut(&current).unwrap().visited = true;
            visited += 1;
            // println!("visited: {} nodes", visited);
            // println!("unvisi: {:?} nodes", unvisited);
            unvisited.sort_by(|a, b| dists[b].dist.cmp(&dists[a].dist));
        }
        dists
    }

    fn is_open(&self, pos: &Pos, include_type: char) -> bool {
        if pos.row < self.rows as i32 && pos.row > -1 && pos.col < self.cols as i32 && pos.col > -1
        {
            match self.map[pos.row as usize][pos.col as usize] {
                Space::Unit(uid) => {
                    return self.units[uid].unit_type == include_type;
                }
                Space::Wall => return false,
                Space::Empty => return true,
            }
        }
        false
    }

    fn surrounding(&self, pos: &Pos, include_type: char) -> Vec<Pos> {
        let mut surround = Vec::new();
        let above = Pos {
            col: pos.col,
            row: pos.row - 1,
        };
        if self.is_open(&above, include_type) {
            surround.push(above);
        }
        let below = Pos {
            col: pos.col,
            row: pos.row + 1,
        };
        if self.is_open(&below, include_type) {
            surround.push(below);
        }
        let left = Pos {
            col: pos.col - 1,
            row: pos.row,
        };
        if self.is_open(&left, include_type) {
            surround.push(left);
        }
        let right = Pos {
            col: pos.col + 1,
            row: pos.row,
        };
        if self.is_open(&right, include_type) {
            surround.push(right);
        }
        surround
    }
}

impl FromStr for Game {
    type Err = Box<dyn std::error::Error>;

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        let mut units = vec![];
        let mut map = vec![];
        for (r_idx, line) in s.lines().enumerate() {
            let mut row = vec![];
            for (c_idx, c) in line.chars().enumerate() {
                match c {
                    '#' => row.push(Space::Wall),
                    'E' | 'G' => {
                        row.push(Space::Unit(units.len()));
                        units.push(Unit {
                            unit_type: c,
                            pos: RefCell::new(Pos {
                                row: r_idx as i32,
                                col: c_idx as i32,
                            }),
                            idx: units.len(),
                            hp: RefCell::new(200),
                        });
                    }
                    '.' => row.push(Space::Empty),
                    _ => panic!("Game can't handle {}", c),
                }
            }
            map.push(row);
        }
        let rows = map.len();
        let cols = map[0].len();

        Ok(Self {
            units,
            map,
            rows,
            cols,
            round: 0,
            elf_damage: 3,
        })
    }
}

impl ToString for Game {
    fn to_string(&self) -> String {
        let mut st = String::with_capacity(self.rows * self.cols * 2);

        for r in 0..self.rows {
            for c in 0..self.cols {
                let c = match self.map[r][c] {
                    Space::Empty => '.',
                    Space::Unit(uid) => self.units[uid].unit_type,
                    Space::Wall => '#',
                };
                st.push(c);
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
    fn test_dists() {
        let input = include_str!("test_path.txt");
        let game: Game = input.parse().unwrap();
        let dists = game.dists(&Pos { row: 0, col: 0 });
        println!("{:#?}, ", dists);
        assert_eq!(dists[&Pos { row: 0, col: 0 }].dist, 0);
        assert_eq!(dists[&Pos { row: 0, col: 2 }].dist, 2);
        assert_eq!(dists[&Pos { row: 2, col: 0 }].dist, 8);
    }
    #[test]
    fn test_move() {
        let input = include_str!("test_move.txt");
        let mut game: Game = input.parse().unwrap();
        let dists = game.dists(&Pos { row: 0, col: 0 });
        println!("{:#?}, ", dists);
        let elf = &game.units[0];
        let new_pos = elf.do_move(&game); // need to split up unit structure from game structure
        assert_eq!(new_pos.unwrap(), Pos { row: 0, col: 1 });
    }
    #[test]
    fn test_example() {
        let input = include_str!("test.txt");
        let mut game: Game = input.parse().unwrap();
        assert_eq!(game.to_string(), input);
        println!("{}", game.to_string());
        game.do_round();
        let round1 = include_str!("test_round_1.txt");
        println!("{}", game.to_string());
        println!("{}", round1.to_string());
        assert_eq!(game.to_string(), round1);
        // println!("game: \n{}", game.to_string());
    }

    #[test]
    fn test_1() {
        let input = include_str!("test_battle.txt");
        let mut game: Game = input.parse().unwrap();
        println!("{}", game.to_string());
        for i in 0..100 {
            game.do_round();
            if game.winner() {
                break;
            }
        }
        assert_eq!(game.score(), 27730);
    }

    #[test]
    fn test_2() {
        let input = include_str!("test_battle2.txt");
        let mut game: Game = input.parse().unwrap();
        println!("{}", game.to_string());
        for i in 0..100 {
            game.do_round();
            if game.winner() {
                break;
            }
        }
        assert_eq!(game.score(), 36334);
    }
    #[test]
    fn test_3() {
        let input = include_str!("test_battle3.txt");
        let mut game: Game = input.parse().unwrap();
        println!("{}", game.to_string());
        for i in 0..100 {
            game.do_round();
            if game.winner() {
                break;
            }
        }
        assert_eq!(game.score(), 39514);
    }
    #[test]
    fn test_4() {
        let input = include_str!("test_battle4.txt");
        let mut game: Game = input.parse().unwrap();
        println!("{}", game.to_string());
        for i in 0..100 {
            game.do_round();
            if game.winner() {
                break;
            }
        }
        assert_eq!(game.score(), 18740);
    }
    #[test]
    fn test_elf_win() {
        let input = include_str!("test_elf.txt");
        let res = part_b(input);
        assert_eq!(res, 1140);
    }
    #[test]
    fn test_elf_win2() {
        let input = include_str!("test_elf2.txt");
        let res = part_b(input);
        assert_eq!(res, 6474)
    }
    #[test]
    fn test_elf_win3() {
        let input = include_str!("test_elf3.txt");
        let res = part_b(input);
        assert_eq!(res, 3478)
    }
}
