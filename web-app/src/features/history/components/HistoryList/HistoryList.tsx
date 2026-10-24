"use client";

import { getI18n } from "@shared/helpers";
import { GridList } from "react-aria-components";
import type { GetSnapshotsResponse } from "../../api/history.schemas";
import { HISTORY_I18N } from "../../history-i18n";
import { useHistoryPage } from "../../use-history-page";
import EstimateCard from "./EstimateCard";
import SnapshotCard from "./SnapshotCard";

interface HistoryListProps {
	history: GetSnapshotsResponse;
	language: string;
	className?: string;
}

/** Cards of the history page on a phone and a portrait tablet, with labelled actions under each card. */
const HistoryList = ({
	history,
	language,
	className = "",
}: Readonly<HistoryListProps>) => {
	const t = getI18n(language, HISTORY_I18N);
	const page = useHistoryPage(history);
	const { currency } = history;

	return (
		<GridList
			aria-label={t("history")}
			items={page.rows}
			className={`grid gap-2 ${className}`}
		>
			{(row) =>
				row.kind === "snapshot" ? (
					<SnapshotCard
						snapshot={row}
						currency={currency}
						language={language}
					/>
				) : (
					<EstimateCard
						estimate={row}
						currency={currency}
						language={language}
					/>
				)
			}
		</GridList>
	);
};

export default HistoryList;
