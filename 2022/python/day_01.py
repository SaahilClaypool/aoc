
def part_a(text: str):
    elfs = []
    cur_elf = []
    for i in text.split('\n'):
        if len(i) == 0:
            elfs.append(cur_elf)
            cur_elf = []
        else:
            cur_elf.append(int(i))

    return max(map(sum, elfs))


def part_b(text):
    """
    sum of top three elves
    """
    elfs = []
    cur_elf = []
    for i in text.split('\n'):
        if len(i) == 0:
            elfs.append(cur_elf)
            cur_elf = []
        else:
            cur_elf.append(int(i))
    elfs.append(cur_elf)

    calories = list(sorted(map(sum, elfs), reverse=True))
    return sum(calories[:3])


sample = """1000
2000
3000

4000

5000
6000

7000
8000
9000

10000"""

ans = 24000


print(f"A Sample: {part_a(sample)}")
with open('../inputs/day_01.txt') as f:
    print(f"A Answer: {part_a(f.read())}")

print(f"B Sample: {part_b(sample)}")
with open('../inputs/day_01.txt') as f:
    print(f"B Answer: {part_b(f.read())}")
