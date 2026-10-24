"use client";

import { formatMoney, formatSnapshotMonth } from "@shared/helpers";
import { Cell, Row } from "react-aria-components";
import type { EstimateResponse } from "../../api/history.schemas";
import EstimateTag from "../EstimateTag";
import TableActions from "./TableActions";
import {
	CELL_CLASSES,
	NUMBER_CELL_CLASSES,
	ROW_CLASSES,
} from "./table-classes";

interface EstimateRowProps {
	estimate: EstimateResponse;
	currency: string;
	language: string;
}

/** Row of an estimate from onboarding: only its amounts, without an index; it can be changed, not deleted. */
const EstimateRow = ({
	estimate,
	currency,
	language,
}: Readonly<EstimateRowProps>) => {
	const month = formatSnapshotMonth(estimate.year, estimate.month, language);

	return (
		<Row
			id={estimate.id}
			textValue={month}
			className={`text-fg-muted ${ROW_CLASSES}`}
		>
			<Cell className={CELL_CLASSES}>
				<span className="grid gap-1">
					<span className="font-semibold text-fg capitalize">{month}</span>
					<EstimateTag language={language} />
				</span>
			</Cell>
			<Cell className={NUMBER_CELL_CLASSES}>—</Cell>
			<Cell className={CELL_CLASSES}>—</Cell>
			<Cell className={NUMBER_CELL_CLASSES}>—</Cell>
			<Cell className={NUMBER_CELL_CLASSES}>
				{formatMoney(estimate.expenses, currency, language)}
			</Cell>
			<Cell className={NUMBER_CELL_CLASSES}>
				{formatMoney(estimate.saved, currency, language)}
			</Cell>
			<Cell className={CELL_CLASSES}>
				<TableActions
					year={estimate.year}
					month={estimate.month}
					monthName={month}
					language={language}
				/>
			</Cell>
		</Row>
	);
};

export default EstimateRow;
