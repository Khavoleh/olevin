/** Rows of one page with the 1-based positions of its first and last row. */
export interface Page<T> {
	rows: T[];
	page: number;
	pageCount: number;
	from: number;
	to: number;
	total: number;
}

/** Cuts the page out of the rows, moving a page past the end back to the last one. */
export const paginate = <T>(
	rows: T[],
	page: number,
	pageSize: number,
): Page<T> => {
	const pageCount = Math.max(1, Math.ceil(rows.length / pageSize));
	const current = Math.min(Math.max(page, 1), pageCount);
	const start = (current - 1) * pageSize;
	const pageRows = rows.slice(start, start + pageSize);

	return {
		rows: pageRows,
		page: current,
		pageCount,
		from: pageRows.length ? start + 1 : 0,
		to: start + pageRows.length,
		total: rows.length,
	};
};
