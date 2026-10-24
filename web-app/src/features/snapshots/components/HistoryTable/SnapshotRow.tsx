"use client";

import { ZoneBadge } from "@shared/components";
import { formatIndex, formatMoney, formatSnapshotMonth } from "@shared/helpers";
import { Cell, Row } from "react-aria-components";
import type { SnapshotResponse } from "../../api/snapshots.schemas";
import SnapshotChange from "../SnapshotChange";
import TableActions from "./TableActions";
import {
	CELL_CLASSES,
	NUMBER_CELL_CLASSES,
	ROW_CLASSES,
} from "./table-classes";

interface SnapshotRowProps {
	snapshot: SnapshotResponse;
	currency: string;
	language: string;
}

/** Row of an actual snapshot: its index, zone, change and amounts. */
const SnapshotRow = ({
	snapshot,
	currency,
	language,
}: Readonly<SnapshotRowProps>) => {
	const month = formatSnapshotMonth(snapshot.year, snapshot.month, language);

	return (
		<Row
			id={snapshot.id}
			textValue={month}
			className={`text-fg ${ROW_CLASSES}`}
		>
			<Cell className={`${CELL_CLASSES} font-semibold capitalize`}>
				{month}
			</Cell>
			<Cell className={`${NUMBER_CELL_CLASSES} font-semibold`}>
				{formatIndex(snapshot.index, language)}
			</Cell>
			<Cell className={CELL_CLASSES}>
				<ZoneBadge zone={snapshot.zone} language={language} size="sm" />
			</Cell>
			<Cell className={NUMBER_CELL_CLASSES}>
				<SnapshotChange change={snapshot.change} language={language} />
			</Cell>
			<Cell className={NUMBER_CELL_CLASSES}>
				{formatMoney(snapshot.expenses, currency, language)}
			</Cell>
			<Cell className={NUMBER_CELL_CLASSES}>
				{formatMoney(snapshot.saved, currency, language)}
			</Cell>
			<Cell className={CELL_CLASSES}>
				<TableActions
					year={snapshot.year}
					month={snapshot.month}
					monthName={month}
					language={language}
					canDelete
				/>
			</Cell>
		</Row>
	);
};

export default SnapshotRow;
