"use client";

import { CalendarBlankIcon } from "@phosphor-icons/react";
import { Heading, Text } from "@shared/components";
import { getI18n } from "@shared/helpers";
import { formatDayMonth } from "../helpers/format-day-month/format-day-month";
import { formatSnapshotMonth } from "../helpers/format-snapshot-month/format-snapshot-month";
import { useHomeStore } from "../home-store";
import { HOME_I18N } from "../i18n/home-i18n";

interface OverviewHeaderProps {
	language: string;
}

/** Title of the overview with the month of the latest snapshot and the date of the next one. */
const OverviewHeader = ({ language }: Readonly<OverviewHeaderProps>) => {
	const t = getI18n(language, HOME_I18N);
	const index = useHomeStore((state) => state.index);

	return (
		<div className="flex flex-wrap items-end justify-between gap-3">
			<div className="grid gap-0.5">
				<Text variant="eyebrow">
					{index ? (
						`${t("snapshot_for")} ${formatSnapshotMonth(index.year, index.month, language)}`
					) : (
						<span
							aria-hidden
							className="block h-4 w-40 rounded-sm bg-skeleton"
						/>
					)}
				</Text>
				<Heading level={1}>{t("title")}</Heading>
			</div>

			<Text variant="hint" className="flex items-center gap-1.5">
				<CalendarBlankIcon size={16} />
				{index ? (
					`${t("next_snapshot")} ${formatDayMonth(index.nextSnapshotDate, language)}`
				) : (
					<span aria-hidden className="block h-4 w-40 rounded-sm bg-skeleton" />
				)}
			</Text>
		</div>
	);
};

export default OverviewHeader;
