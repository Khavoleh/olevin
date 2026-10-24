"use client";

import { getI18n } from "@shared/helpers";
import { Column, Table, TableBody, TableHeader } from "react-aria-components";
import type { GetSnapshotsResponse } from "../../api/snapshots.schemas";
import { SNAPSHOTS_I18N } from "../../snapshots-i18n";
import { useHistoryPage } from "../../use-history-page";
import EstimateRow from "./EstimateRow";
import SnapshotRow from "./SnapshotRow";

interface HistoryTableProps {
	history: GetSnapshotsResponse;
	language: string;
	className?: string;
}

const HEAD_CLASSES =
	"whitespace-nowrap bg-surface-sunken px-3 py-3 font-semibold -outline-offset-2 text-fg-muted text-xs uppercase tracking-label";

const COLUMN_CLASSES = `${HEAD_CLASSES} text-left`;

const ACTIONS_COLUMN_CLASSES = `${HEAD_CLASSES} text-center`;

/** Card-like table whose corner cells follow its rounding, so their focus rings are not cut. */
const TABLE_CLASSES =
	"w-full border-separate border-spacing-0 rounded-xl border border-border bg-surface text-sm shadow-card [&_th:first-child]:rounded-tl-[calc(var(--radius-xl)-1px)] [&_th:last-child]:rounded-tr-[calc(var(--radius-xl)-1px)] [&_tr:last-child>td:first-child]:rounded-bl-[calc(var(--radius-xl)-1px)] [&_tr:last-child>td:last-child]:rounded-br-[calc(var(--radius-xl)-1px)]";

/** Table of the history page on a tablet and a laptop, with icon actions in the last column. */
const HistoryTable = ({
	history,
	language,
	className = "",
}: Readonly<HistoryTableProps>) => {
	const t = getI18n(language, SNAPSHOTS_I18N);
	const page = useHistoryPage(history);
	const { currency } = history;

	return (
		<div className={`overflow-x-auto pb-1 ${className}`}>
			<Table aria-label={t("history")} className={TABLE_CLASSES}>
				<TableHeader>
					<Column isRowHeader className={COLUMN_CLASSES}>
						{t("month")}
					</Column>
					<Column className={COLUMN_CLASSES}>{t("index")}</Column>
					<Column className={COLUMN_CLASSES}>{t("zone")}</Column>
					<Column className={COLUMN_CLASSES}>{t("change")}</Column>
					<Column className={COLUMN_CLASSES}>{t("expenses")}</Column>
					<Column className={COLUMN_CLASSES}>{t("saved")}</Column>
					<Column className={ACTIONS_COLUMN_CLASSES}>{t("actions")}</Column>
				</TableHeader>
				<TableBody items={page.rows}>
					{(row) =>
						row.kind === "snapshot" ? (
							<SnapshotRow
								snapshot={row}
								currency={currency}
								language={language}
							/>
						) : (
							<EstimateRow
								estimate={row}
								currency={currency}
								language={language}
							/>
						)
					}
				</TableBody>
			</Table>
		</div>
	);
};

export default HistoryTable;
