# Slot Math & RNG — Console Practice Tasks

A progressive set of tiny console-app exercises designed to build intuition for 
slot machine math, RNG, probability, paylines, RTP, volatility, and bonus mechanics — one concept at a time.

> **How to use this file**
> - Pick a task, open a fresh `Program.cs` (or one big console project with a menu — your call), and solve it.
> - Don't peek at later tasks until you finish the current one. Each one assumes you understand the previous.
> - Every task lists: **Goal**, **Input/Output**, **Hint**, and **Why it matters** (how it maps to a real slot engine).
> - Stuck? Re-read the *Why it matters* — it usually points at the right mental model.

Recommended language: C# (matches your backend), but pseudocode/JS/Python all work.

---

## Tier 0 — Warm-up: random numbers

### Task 0.1 — Roll a die
**Goal:** Print a random integer between 1 and 6.
**Hint:** `new Random().Next(1, 7)` (upper bound is exclusive in C#).
**Why it matters:** Every slot spin is, at heart, "roll some dice." Get comfortable with the API and the off-by-one trap of exclusive upper bounds.

### Task 0.2 — Roll 10,000 dice, count each face
**Goal:** Roll a d6 ten thousand times, print how many times each face came up.
**Expected output (roughly):** Each face ~1666 times.
**Why it matters:** This is your first **empirical probability** check. Real slot QA does the same thing with millions of spins.

### Task 0.3 — Seeded RNG
**Goal:** Run Task 0.2 twice with `new Random(42)` and confirm the two runs produce **identical** counts.
**Why it matters:** Reproducibility. Provably-fair slots and unit tests both rely on seedable RNG. `System.Random` is fine for learning; production uses `RandomNumberGenerator` (cryptographic).

### Task 0.4 — Cryptographic RNG
**Goal:** Use `System.Security.Cryptography.RandomNumberGenerator.GetInt32(1, 7)` and roll 10k dice. Compare distribution to Task 0.2.
**Why it matters:** Regulated jurisdictions require CSPRNG for real-money gambling. `Random` is predictable and trivially exploitable.

---

## Tier 1 — Weighted picks (the heart of slot math)

### Task 1.1 — Pick from a list
**Goal:** Given symbols `["Cherry", "Lemon", "Bell", "Seven"]`, print one at random with equal probability.
**Why it matters:** Reels are *just* weighted lists of symbols. This is the simplest possible reel.

### Task 1.2 — Weighted pick
**Goal:** Given `[("Cherry", 50), ("Lemon", 30), ("Bell", 15), ("Seven", 5)]` (weights), pick one symbol so its frequency matches the weights. Run 10k times to verify.
**Hint:** Sum the weights (100). Roll `Next(0, 100)`. Walk the list subtracting weights until the roll is < 0.
**Why it matters:** **This is how slots actually work.** Sevens are rare because their *weight* is small — not because the math is fancy. Tattoo this on your brain.

### Task 1.3 — Refactor: `WeightedRandom<T>` helper
**Goal:** Build a reusable class: `new WeightedRandom<string>().Add("Cherry", 50)...Pick()`.
**Why it matters:** You'll reuse this everywhere — reel strips, bonus picks, free-spin triggers.

### Task 1.4 — Verify weights with a histogram
**Goal:** Pick 1,000,000 times from Task 1.2's distribution. Print a text histogram:
```
Cherry  ████████████████████████████ 49.97%
Lemon   █████████████████            29.98%
Bell    █████████                    15.04%
Seven   ███                           5.01%
```
**Why it matters:** You'll do this constantly when balancing a game. "Did my change make sevens too common?" — histogram tells you in 2 seconds.

---

## Tier 2 — Reels and reel strips

### Task 2.1 — Single reel strip
**Goal:** A reel strip is an array of symbols, e.g. `[Cherry, Cherry, Lemon, Bell, Cherry, Seven, Lemon, ...]` (length 30+). Pick a random index — the symbol at that index is the reel's "stop."
**Why it matters:** This is the **physical model** — a strip of stickers on a wheel. Probabilities are now driven by *how many of each symbol appear on the strip*, not abstract weights. Every land-based and most online slots work this way.

### Task 2.2 — 5 reels, 1 row (single payline)
**Goal:** Make 5 different reel strips. Spin all 5 (pick one symbol from each). Print the resulting line, e.g. `[Cherry, Cherry, Cherry, Lemon, Bell]`.
**Why it matters:** A "spin" is just N independent picks, one per reel. That's it. Everything else is evaluation.

### Task 2.3 — Detect a 3-of-a-kind win (left-to-right)
**Goal:** Given a 5-symbol line, return `true` if the first 3+ symbols (starting from the left) are the same.
**Examples:**
- `[Cherry, Cherry, Cherry, Lemon, Bell]` → true (3 Cherries)
- `[Cherry, Cherry, Lemon, Cherry, Cherry]` → false (only 2 in a row from the left)
- `[Bell, Bell, Bell, Bell, Bell]` → true (5 of a kind!)
**Why it matters:** Standard slot rule: wins pay **left-to-right starting from reel 1**. Break this rule and players will be very confused.

### Task 2.4 — Pay table lookup
**Goal:** Define a paytable, e.g.:
```
Seven:   3x→50,  4x→200, 5x→1000
Bell:    3x→20,  4x→80,  5x→300
Lemon:   3x→5,   4x→15,  5x→50
Cherry:  3x→2,   4x→8,   5x→25
```
Given a line, return the win amount (longest match wins).
**Why it matters:** The paytable is the contract with the player. Most balancing happens here, not in code.

### Task 2.5 — 1,000,000 spin simulation, compute RTP
**Goal:** Spin Task 2.4's machine 1,000,000 times with bet=1. Sum total wins. Print `RTP = totalWins / totalBet * 100%`.
**Target:** A real slot is usually 92–97%. Tune your weights until you hit ~95%.
**Why it matters:** **RTP is the single most important number in slot math.** It's regulated, audited, and advertised. If you can compute it and tune it, you understand 80% of slot math.

---

## Tier 3 — Paylines

### Task 3.1 — 3x5 grid spin
**Goal:** A real slot screen is 3 rows × 5 columns. Each reel produces 3 visible symbols. Generate and print a 3x5 grid.
**Hint:** Pick a random index `i` on the strip; the visible window is `strip[i], strip[i+1], strip[i+2]` (wrap around).
**Why it matters:** This is what the player actually sees. Wrap-around is where beginners trip up — handle the modulo.

### Task 3.2 — Define paylines
**Goal:** A payline is an array of 5 row-indices, one per reel. Define these classics:
- Line 1: `[1, 1, 1, 1, 1]` — middle row straight across
- Line 2: `[0, 0, 0, 0, 0]` — top row
- Line 3: `[2, 2, 2, 2, 2]` — bottom row
- Line 4: `[0, 1, 2, 1, 0]` — V-shape
- Line 5: `[2, 1, 0, 1, 2]` — inverted V
**Why it matters:** Paylines are *just patterns*. The same evaluator works for 1, 5, 20, or 50 paylines.

### Task 3.3 — Evaluate all 5 paylines
**Goal:** Spin a 3x5 grid, then for each payline extract the 5 symbols on it and run your Task 2.4 evaluator. Sum total win.
**Why it matters:** Multi-line wins are independent — compute each payline, sum results. Beginners often try to be clever; don't.

### Task 3.4 — Bet-per-line scaling
**Goal:** Player bets `betPerLine = 1` and plays all 5 lines → total bet = 5. Wins are multiplied by `betPerLine`. Re-run RTP simulation.
**Verify:** RTP should be unchanged from Task 2.5 (it's a percentage — bet size is irrelevant).
**Why it matters:** A common bug: scaling wins by total bet instead of bet-per-line. Get this right once and never think about it again.

---

## Tier 4 — Wilds, scatters, and special symbols

### Task 4.1 — Wild symbol
**Goal:** Add a `Wild` symbol that substitutes for any regular symbol (not scatter). `[Cherry, Wild, Cherry, Lemon, Bell]` should count as 3 Cherries.
**Why it matters:** Wilds are the most common "special." Almost every modern slot has them.

### Task 4.2 — Wild-only line
**Goal:** `[Wild, Wild, Wild, Wild, Wild]` should pay as 5x the highest-paying symbol (or 5x Wild if Wild has its own paytable entry — your call, document the choice).
**Why it matters:** Edge cases are where bugs hide. Decide the rule, write a test, move on.

### Task 4.3 — Scatter symbol
**Goal:** Add a `Scatter` symbol. Unlike regular symbols, it pays based on **count anywhere on the screen**, not adjacency on a payline. 3 scatters anywhere → win 5x bet.
**Why it matters:** Scatters break the payline model. Their evaluation is independent of paylines — it scans the whole 3x5 grid.

### Task 4.4 — Free-spins trigger
**Goal:** 3+ scatters → award 10 free spins. Track free-spin count, run them automatically (no extra bet), accumulate winnings.
**Why it matters:** Bonus features are usually "more spins under modified rules." Once you can trigger and track them, the rest is variations.

### Task 4.5 — Free-spins with multiplier
**Goal:** During free spins, all wins are 2x.
**Why it matters:** Multipliers are the easiest "feature" to add huge variance. We'll quantify that next.

---

## Tier 5 — Volatility and distribution

### Task 5.1 — Hit frequency
**Goal:** In your million-spin sim, count how often *any* win occurs. Print `hitFrequency = winningSpins / totalSpins`.
**Typical range:** 20–35%. Below 20% feels punishing; above 40% feels twitchy.
**Why it matters:** RTP tells you the *average*. Hit frequency tells you how often the player gets *any* dopamine hit.

### Task 5.2 — Volatility / variance
**Goal:** Compute the **standard deviation** of per-spin payout. Print mean, stddev, and stddev/mean (coefficient of variation).
**Why it matters:** Two slots with identical 96% RTP can feel completely different. A "low-volatility" slot pays small wins often; a "high-volatility" slot pays rarely but huge. Stddev is the math.

### Task 5.3 — Win distribution buckets
**Goal:** Bucket every win into ranges (0x bet, 0–1x, 1–5x, 5–20x, 20–100x, 100x+) and print the percentage of spins in each.
**Why it matters:** Players don't think in averages — they remember the distribution. "I lost 50 times in a row then hit 80x" is a *good* memory because of the shape of the distribution, not the mean.

### Task 5.4 — Max-win tracker
**Goal:** Track the largest single-spin payout in 10 million spins. Compare across different paytables.
**Why it matters:** Max win is a marketing number ("Win up to 5000x your bet!") and a regulatory number (some jurisdictions cap it).

---

## Tier 6 — Math model: target an RTP

### Task 6.1 — Forward calculation
**Goal:** Given reel strips and a paytable, compute **theoretical RTP analytically** (not via simulation). For each possible 5-symbol combination on each payline, calculate `probability × payout`, sum them all.
**Hint:** P(symbol on reel) = `count(symbol) / strip.Length`. Combinations multiply across reels (independence).
**Why it matters:** A million-spin sim gives you ~99.5% accurate RTP. Math gives you 100%. Regulators want the math.

### Task 6.2 — Compare math vs simulation
**Goal:** Run Task 6.1's math and Task 2.5's simulation. Should agree within ~0.1%.
**Why it matters:** If they disagree, **one of them is wrong** — and finding which one is a fantastic debugging exercise.

### Task 6.3 — Tune to a target RTP
**Goal:** Pick a target (say 95.5%). Adjust reel strip composition (add/remove symbol counts) until math says 95.5% ± 0.05%.
**Why it matters:** This is **the actual job of a slot mathematician.** Do this once and you'll respect them.

---

## Tier 7 — RNG quality and fairness

### Task 7.1 — Two RNG implementations side-by-side
**Goal:** Compare `System.Random` vs `RandomNumberGenerator` over 10M spins. RTP should match (within noise).
**Why it matters:** Output statistics are the same; the difference is **predictability**, not fairness on average.

### Task 7.2 — Predict the next `System.Random`
**Goal:** Seed `Random` with a known value, draw 10 numbers, then re-seed with the same value and confirm the next 10 are identical.
**Why it matters:** Demonstrates *why* `System.Random` is unfit for production gambling. With ~100 observed outputs, an attacker can recover the seed.

### Task 7.3 — Provably-fair commit-reveal
**Goal:** Server picks a secret seed, hashes it (SHA-256), and shows the hash to the player **before** the spin. After the spin, server reveals the seed. Player can verify `SHA256(seed) == hash` and recompute the spin themselves.
**Why it matters:** Crypto casinos run on this primitive. It's also a great way to internalize that "fairness" is a *protocol*, not a vibe.

---

## Tier 8 — Engine architecture (mini-project)

### Task 8.1 — Separate config from engine
**Goal:** Move all weights, paylines, and paytable into a JSON file. Engine loads JSON at startup. Changing the config changes the game with **zero code changes**.
**Why it matters:** This is how real slot studios ship 50 games a year — the engine is fixed; each game is a config.

### Task 8.2 — Deterministic replay
**Goal:** Save every spin's `(seed, gridResult, totalWin)` to a log. Add a "replay" mode that re-runs spins from the seed and verifies the result matches the log.
**Why it matters:** Audit, debugging, dispute resolution. "The player claims they won X but our log says Y" — replay settles it.

### Task 8.3 — Balance/wallet integration
**Goal:** Player starts with 1000 credits. Each spin debits the bet, credits the win. Refuse to spin if balance < bet. Track session profit/loss.
**Why it matters:** This is the boundary between "math model" and "real product." Most production bugs live exactly here (double-debits, race conditions, currency precision).

### Task 8.4 — Session report
**Goal:** After N spins, print: total bet, total win, session RTP, biggest single win, hit frequency, longest losing streak.
**Why it matters:** Player-facing transparency, but also your debugging dashboard.

---

## Tier 9 — Stretch goals

### Task 9.1 — Cascading/avalanche reels
**Goal:** Winning symbols disappear, symbols above fall down, new symbols fill the top, re-evaluate. Repeat until no win.
**Real example:** *Gonzo's Quest*, *Sweet Bonanza*.
**Why it matters:** Same math primitives, very different feel. Forces you to think recursively about a single "spin event."

### Task 9.2 — Cluster pays
**Goal:** Win when 5+ of the same symbol are connected (orthogonally adjacent), regardless of paylines.
**Real example:** *Aloha Cluster Pays*, *Sweet Bonanza*.
**Hint:** Flood fill / connected components.
**Why it matters:** Paylines are one paradigm; cluster pays are another. Both reduce to "find groups of matching symbols."

### Task 9.3 — Megaways
**Goal:** Each reel has a *random number of visible rows* (2–7) per spin. Number of ways = product of row counts. Win = same symbol left-to-right on any reel position.
**Real example:** *Bonanza Megaways*.
**Why it matters:** Hot mechanic, surprisingly elegant once you stop counting paylines and start counting *paths*.

### Task 9.4 — Bonus pick game
**Goal:** Trigger a bonus screen with 12 hidden prizes. Player picks until they reveal a "Collect" symbol. Show distribution of bonus payouts.
**Why it matters:** Most slots have a non-spin bonus mode. The math is just a weighted distribution (Tier 1!) wrapped in UI.

### Task 9.5 — Buy-feature
**Goal:** Player can pay 100x bet to skip directly to free spins. Verify that buy-feature RTP matches the natural-trigger RTP.
**Why it matters:** Real product feature in many modern slots. Also a great way to find off-by-one errors in your free-spin code.

---

## When you're done

If you can knock out Tiers 0–6 and one task from Tier 9, you understand slot math at the level of a junior slot mathematician. Tiers 7–8 round you out as an engineer. Don't rush — Tier 2's RTP simulation is the single highest-leverage exercise in the whole list. Sit with it until the numbers feel obvious.

Then come back to `SlotEngine.API` / `SlotEngine.Front` and the existing code will read like English.
