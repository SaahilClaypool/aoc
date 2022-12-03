fn main() {
    let mut s = State::new();
    let input = 165061;
    for i in 0..100 * input{// this should generate enough 
        s.step();
    }
    let res = s.after_len(input);
    println!("after {} is {}", 165061, res);
    let first = s.left_of(input);
    println!("first occurence of {} after {}", input, first);
}

struct State {
    one: usize,
    two: usize,
    recipes: Vec<usize>,
    count: usize,
}

impl State {
    fn new() -> State {
        State {
            one: 0,
            two: 1,
            recipes: vec![3, 7],
            count: 0,
        }
    }

    fn to_digits(inp: usize) -> Vec<usize> {
        let mut inp = inp;
        let mut digits = Vec::new();
        while inp > 0 {
            digits.push(inp % 10);
            inp = inp / 10;
        }
        digits.reverse();
        digits
    }

    fn left_of(&self, inp: usize) -> usize {
        let digits = Self::to_digits(inp);
        let mut idx = 0;
        while idx < self.recipes.len() - digits.len() {
            let mut cur_idx = 0;
            let mut r_idx = idx;
            while digits[cur_idx] == self.recipes[r_idx] {
                cur_idx += 1;
                r_idx += 1;
                if cur_idx == digits.len() {
                    return idx;
                }
            }
            idx+=1;
        }
        idx
    }

    fn slice_to_str(&self, start: usize, end: usize) -> String{
        self.recipes[start..end].iter().map(|dig| dig.to_string().chars().nth(0).unwrap()).collect()
    }

    fn after_len(&self, l: usize) -> String {
        self.slice_to_str(l, l+10)
    }

    fn step(&mut self) {
        self.count += 1;
        let sum = self.recipes[self.one] + self.recipes[self.two];
        let dig1 = sum / 10;
        let dig2 = sum % 10;
        if dig1 != 0 {
            self.recipes.push(dig1);
        }
        self.recipes.push(dig2);
        self.update_index();
    }
    fn update_index(&mut self) {
        // update index.
        // index = current + 1 % size
        self.one = (self.one + self.recipes[self.one] + 1) % self.recipes.len();
        self.two = (self.two + self.recipes[self.two] + 1) % self.recipes.len();
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test1() {
        let mut s = State::new();
        s.step();
        assert_eq!(s.recipes, vec![3, 7, 1, 0]);
        assert_eq!(s.one, 0);
        assert_eq!(s.two, 1);
    }
    #[test]
    fn test2() {
        let mut s = State::new();
        s.step();
        s.step();
        assert_eq!(s.recipes, vec![3, 7, 1, 0, 1, 0]);
        assert_eq!(s.one, 4);
        assert_eq!(s.two, 3);
    }
    #[test]
    fn test3() {
        let mut s = State::new();
        s.step();
        s.step();
        s.step();
        assert_eq!(s.recipes, vec![3, 7, 1, 0, 1, 0, 1]);
        assert_eq!(s.one, 6);
        assert_eq!(s.two, 4);
    }

    #[test]
    fn test_after9() {
        let mut s = State::new();
        for i in 0..100 {
            s.step();
        }
        assert_eq!("5158916779", s.after_len(9))
    }

    #[test]
    fn test_appear() {
        let input = 51589;
        let mut s = State::new();
        for i in 0..10000 {
            s.step();
        }
        let l = s.left_of(input);
        assert_eq!(l, 9);
    }
    #[test]
    fn test_appear2() {
        let input = 59414;
        let mut s = State::new();
        for i in 0..10000 {
            s.step();
        }
        let l = s.left_of(input);
        assert_eq!(l, 2018);
    }

    #[test]
    fn to_digits() {
        let dig = State::to_digits(59414);
        assert_eq!(dig, vec![5, 9, 4, 1, 4]);
    }
}
