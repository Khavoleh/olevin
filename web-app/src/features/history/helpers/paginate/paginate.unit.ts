import { describe, expect, it } from "vitest";
import { paginate } from "./paginate";

const ROWS = Array.from({ length: 24 }, (_, index) => index + 1);

describe("paginate", () => {
	it("should cut the first page", () => {
		expect(paginate(ROWS, 1, 10)).toEqual({
			rows: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
			page: 1,
			pageCount: 3,
			from: 1,
			to: 10,
			total: 24,
		});
	});

	it("should cut a shorter last page", () => {
		expect(paginate(ROWS, 3, 10)).toMatchObject({
			rows: [21, 22, 23, 24],
			from: 21,
			to: 24,
		});
	});

	it("should move a page past the end back to the last one", () => {
		expect(paginate(ROWS, 5, 10).page).toBe(3);
	});

	it("should move a page before the start to the first one", () => {
		expect(paginate(ROWS, 0, 10).page).toBe(1);
	});

	it("should keep one empty page without rows", () => {
		expect(paginate([], 1, 10)).toEqual({
			rows: [],
			page: 1,
			pageCount: 1,
			from: 0,
			to: 0,
			total: 0,
		});
	});
});
