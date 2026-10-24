"use client";

import {
	formatIndex,
	formatMoney,
	formatSnapshotMonth,
	getI18n,
} from "@shared/helpers";
import { GridListItem } from "react-aria-components";
import type { SnapshotResponse } from "../../api/snapshots.schemas";
import { SNAPSHOTS_I18N } from "../../snapshots-i18n";
import SnapshotChange from "../SnapshotChange";
import CardActions from "./CardActions";

interface SnapshotCardProps {
	snapshot: SnapshotResponse;
	currency: string;
	language: string;
}

/** Card of an actual snapshot: its amounts on the left, the index and its change on the right. */
const SnapshotCard = ({
	snapshot,
	currency,
	language,
}: Readonly<SnapshotCardProps>) => {
	const t = getI18n(language, SNAPSHOTS_I18N);
	const month = formatSnapshotMonth(snapshot.year, snapshot.month, language);

	return (
		<GridListItem
			id={snapshot.id}
			textValue={month}
			className="rounded-lg border border-border bg-surface outline-none data-focus-visible:ring-2 data-focus-visible:ring-ring"
		>
			<div className="grid grid-cols-[minmax(0,1fr)_auto] items-center gap-2 p-3.5">
				<div className="min-w-0">
					<p className="font-semibold capitalize">{month}</p>
					<p className="text-fg-muted text-sm tabular-nums">
						<span className="whitespace-nowrap">
							{t("expenses")}{" "}
							{formatMoney(snapshot.expenses, currency, language)}
						</span>{" "}
						·{" "}
						<span className="whitespace-nowrap">
							{t("saved").toLocaleLowerCase(language)}{" "}
							{formatMoney(snapshot.saved, currency, language)}
						</span>
					</p>
				</div>
				<div className="grid justify-items-end gap-1">
					<span className="font-semibold text-xl tabular-nums leading-none">
						{formatIndex(snapshot.index, language)}
					</span>
					<SnapshotChange change={snapshot.change} language={language} />
				</div>
			</div>
			<CardActions
				year={snapshot.year}
				month={snapshot.month}
				monthName={month}
				language={language}
				canDelete
			/>
		</GridListItem>
	);
};

export default SnapshotCard;
