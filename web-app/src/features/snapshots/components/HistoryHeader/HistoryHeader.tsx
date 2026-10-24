"use client";

import { CalendarBlankIcon } from "@phosphor-icons/react";
import { Heading, Text } from "@shared/components";
import { formatDayMonth, getI18n } from "@shared/helpers";
import { getPluralKey } from "../../helpers/get-plural-key/get-plural-key";
import { SNAPSHOTS_I18N } from "../../snapshots-i18n";
import { useSnapshotsStore } from "../../snapshots-store";
import NewSnapshotButton from "./NewSnapshotButton";

interface HistoryHeaderProps {
	language: string;
}

/** Title of the history with the number of snapshots and estimates, the new snapshot button and, on a phone, when it opens. */
const HistoryHeader = ({ language }: Readonly<HistoryHeaderProps>) => {
	const t = getI18n(language, SNAPSHOTS_I18N);
	const history = useSnapshotsStore((state) => state.history);
	const count = (key: string, value: number) =>
		`${value} ${t(getPluralKey(key, value, language))}`;

	return (
		<div className="grid gap-2">
			<div className="flex flex-wrap items-end justify-between gap-3">
				<div className="grid gap-0.5">
					<Text variant="eyebrow">
						{history ? (
							[
								count("snapshots", history.snapshots.length),
								history.estimates.length > 0 &&
									count("estimates", history.estimates.length),
							]
								.filter(Boolean)
								.join(" · ")
						) : (
							<span
								aria-hidden
								className="block h-4 w-40 rounded-sm bg-skeleton"
							/>
						)}
					</Text>
					<Heading level={1}>{t("title")}</Heading>
				</div>

				<NewSnapshotButton language={language} />
			</div>

			{history?.newSnapshotOpensOn && (
				<Text variant="hint" className="flex items-center gap-1.5 md:hidden">
					<CalendarBlankIcon size={16} />
					{`${t("opens_on")} ${formatDayMonth(history.newSnapshotOpensOn, language)}`}
				</Text>
			)}
		</div>
	);
};

export default HistoryHeader;
