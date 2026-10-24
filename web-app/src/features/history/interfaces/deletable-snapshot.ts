/** Snapshot a row can delete, with the number of later snapshots scored again without it. */
export interface DeletableSnapshot {
	id: string;
	laterSnapshots: number;
	isOnly: boolean;
}
