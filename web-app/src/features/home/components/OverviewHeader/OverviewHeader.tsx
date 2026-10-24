import { CalendarBlankIcon } from "@phosphor-icons/react/dist/ssr";
import { Heading, Text } from "@shared/components";
import { getI18n } from "@shared/helpers";
import { Suspense } from "react";
import { HOME_I18N } from "../../i18n/home-i18n";
import NextSnapshotDate from "./NextSnapshotDate";
import SnapshotMonth from "./SnapshotMonth";

interface OverviewHeaderProps {
	language: string;
}

/** Title of the overview with the month of the latest snapshot and the date of the next one. */
const OverviewHeader = ({ language }: Readonly<OverviewHeaderProps>) => {
	const t = getI18n(language, HOME_I18N);

	return (
		<div className="flex flex-wrap items-end justify-between gap-3">
			<div className="grid gap-0.5">
				<Text variant="eyebrow">
					<Suspense
						fallback={
							<span
								aria-hidden
								className="block h-4 w-40 rounded-sm bg-skeleton"
							/>
						}
					>
						<SnapshotMonth language={language} />
					</Suspense>
				</Text>
				<Heading level={1}>{t("title")}</Heading>
			</div>

			<Text variant="hint" className="flex items-center gap-1.5">
				<CalendarBlankIcon size={16} />
				<Suspense
					fallback={
						<span
							aria-hidden
							className="block h-4 w-40 rounded-sm bg-skeleton"
						/>
					}
				>
					<NextSnapshotDate language={language} />
				</Suspense>
			</Text>
		</div>
	);
};

export default OverviewHeader;
