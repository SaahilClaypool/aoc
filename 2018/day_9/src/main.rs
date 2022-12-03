use std::collections::HashSet;
use std::collections::LinkedList;
use std::time::Instant;

fn main() {
    let start = Instant::now();
    let mut g = Game::new(466, 71436);
    let max_score = g.play_game();

    let end = Instant::now();
    println!("Finished in : {} ms", end.duration_since(start).subsec_millis());
    println!("Max score: {}", max_score);

    let start = Instant::now();
    let mut g = Game::new(466, 71436 * 100);
    let max_score = g.play_game();

    let end = Instant::now();
    println!("Finished in : {} ms", end.duration_since(start).subsec_millis());
    println!("Max score: {}", max_score);
}

#[derive(Clone, Copy)]
struct Marble {
    val: u32, 
    left: usize,
    right: usize,
    index: usize
}

struct Game {
    players: Vec<u32>,
    total_marbles: u32,
    current_marble: u32,
    marbles: Vec<Marble>,
    current_player: usize,
    last_placed: usize,
}

impl Game {
    fn new(num_players: u32, num_marbles: u32) -> Self {
        let mut players = vec![];
        for _ in 0..num_players {
            players.push(0)
        }
        // let mut marbles = Vec::with_capacity(num_marbles as usize);
        let mut marbles = Vec::new();
        marbles.push(Marble {val: 0, left: 0, right: 0, index: 0});

        Game {
            players: players,
            total_marbles: num_marbles,
            current_marble: 1,
            marbles: marbles,
            current_player: 1,
            last_placed: 0,
        }
    }

    fn get_index(&self) -> usize {
        ((self.last_placed + 1) as usize % self.active_marbles()) + 1
    }

    fn play_game(&mut self) -> u32 {
        for _ in 0..self.total_marbles {
            self.round();
        }
        self.highest_score()
    }

    fn round(&mut self) {
        if self.current_marble % 23 != 0 {
            let new_index = self.insert_marble(1);
            self.last_placed = new_index;
        } else {
            // keep marble. Add to score
            *self.players.get_mut(self.current_player).unwrap() += self.current_marble;
            let (value, new_index) = self.remove_marble_counter(7);
            *self.players.get_mut(self.current_player).unwrap() += value;
            self.last_placed = new_index;
        }

        self.current_marble += 1;
        self.current_player += 1;
        self.current_player %= self.players.len();
    }

    /// offset from last placed
    fn insert_marble(&mut self, index: usize) -> usize {
        let prior_marble_index = self.get_clockwise(index).index;
        let next_marble_index = self.get_clockwise(index+1).index;
        let new_index = self.marbles.len();
        let new_marble = Marble {
            val: self.current_marble, 
            left: prior_marble_index,
            right: next_marble_index,
            index: new_index,
        };
        self.marbles.push(new_marble);

        let prior_marble = self.marbles.get_mut(prior_marble_index).unwrap();
        prior_marble.right = new_index;
        let next_marble = self.marbles.get_mut(next_marble_index).unwrap();
        next_marble.left = new_index;

        return new_index;
    }

    /// nth from last placed
    fn get_clockwise(&mut self, n: usize) -> &mut Marble {
        let mut cur = self.marbles.get(self.last_placed).unwrap();
        for _ in 0..n {
            cur = self.marbles.get(cur.right).unwrap();
        }
        let cur_index = cur.index;
        self.marbles.get_mut(cur_index).unwrap()
    }

    fn get_counter_clockwise(&mut self, n: usize) -> &mut Marble {
        // get n from the last offset
        let mut cur = self.marbles.get(self.last_placed).unwrap();
        for _ in 0..n {
            cur = self.marbles.get(cur.left).unwrap();
        }
        let cur_index = cur.index;
        self.marbles.get_mut(cur_index).unwrap()
    }

    fn remove_marble_counter(&mut self, remove_index: usize) -> (u32, usize) {
        let marble_val = self.get_counter_clockwise(remove_index).val;
        let next_marble_index = self.get_counter_clockwise(remove_index).right;
        let prev_marble_index = self.get_counter_clockwise(remove_index).left;
        let mut prev_marble = self.marbles.get_mut(prev_marble_index).unwrap();
        prev_marble.right = next_marble_index;
        let mut next_marble = self.marbles.get_mut(next_marble_index).unwrap();
        next_marble.left = prev_marble_index;
        return (marble_val, next_marble_index);
    }

    fn active_marbles(&self) -> usize {
        self.marbles.len()
    }

    fn highest_score(&self) -> u32 {
        self.players.iter().map(|v| *v).max().unwrap()
    }

    fn to_vec(&self) -> Vec<u32> {
        let mut v = Vec::new();
        let mut cur = self.marbles[0];
        v.push(cur.val);
        while cur.right != 0 {
            cur = self.marbles[cur.right];
            v.push(cur.val);
        }
        return v;
    }
}


#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn testind() {
        let g = Game::new(9, 25);
        let ind = g.get_index();
        assert_eq!(ind, 1);
    }

    #[test]
    fn testmarbles() {
        let mut g = Game::new(9, 25);
        g.round();
        assert_eq!(vec![0, 1], g.to_vec());
        g.round();
        assert_eq!(vec![0, 2, 1], g.to_vec());
        g.round();
        assert_eq!(vec![0, 2, 1, 3], g.to_vec());
        g.round();
        assert_eq!(vec![0, 4, 2, 1, 3], g.to_vec());
    }

    #[test]
    fn test_larger() {
        let mut g = Game::new(9, 25);
        for i in 0..23 {
            g.round();
        }
        eprintln!("{:?}", g.to_vec());
        // assert_eq!(g.get_counter_clockwise(1).val, 19);
    }

    #[test]
    fn test_examples() {
        let mut g = Game::new(9, 25);
        assert_eq!(g.play_game(), 32);

        let mut g = Game::new(30, 5807);
        assert_eq!(g.play_game(), 37305)
    }
}
