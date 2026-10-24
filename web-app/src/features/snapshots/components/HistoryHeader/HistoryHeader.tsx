import { Heading, Text } from "@shared/components";
import { getI18n } from "@shared/helpers";
import { Suspense } from "react";
import { SNAPSHOTS_I18N } from "../../snapshots-i18n";
import HistoryCount from "./HistoryCount";
import NewSnapshotButton from "./NewSnapshotButton";
import OpensOnHint from "./OpensOnHint";

interface HistoryHeaderProps {
	language: string;
}

/** Title of the history with the number of snapshots and estimates, the new snapshot button and, on a phone, when it opens. */
const HistoryHeader = ({ language }: Readonly<HistoryHeaderProps>) => {
	const t = getI18n(language, SNAPSHOTS_I18N);

	return (
		<div className="grid gap-2">
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
							<HistoryCount language={language} />
						</Suspense>
					</Text>
					<Heading level={1}>{t("title")}</Heading>
				</div>

				<Suspense
					fallback={
						<span
							aria-hidden
							className="block h-11 w-40 rounded-md bg-skeleton"
						/>
					}
				>
					<NewSnapshotButton language={language} />
				</Suspense>
			</div>

			<Suspense>
				<OpensOnHint language={language} />
			</Suspense>
		</div>
	);
};

export default HistoryHeader;
