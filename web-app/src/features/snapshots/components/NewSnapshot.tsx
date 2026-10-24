import { ErrorToast, Link, Text } from "@shared/components";
import { PAGE_URLS } from "@shared/constants";
import { formatDayMonth, getI18n, getLanguageUrl } from "@shared/helpers";
import { getNewSnapshotMonths } from "../loaders/get-new-snapshot-months";
import { getSavedBaseCurrency } from "../loaders/get-saved-base-currency";
import { SNAPSHOT_FORM_I18N } from "../snapshot-form-i18n";
import NewSnapshotForm from "./NewSnapshotForm/NewSnapshotForm";

interface NewSnapshotProps {
	language: string;
}

/** Form of a new snapshot, or the day the next month opens while every open month has one. */
const NewSnapshot = async ({ language }: Readonly<NewSnapshotProps>) => {
	const t = getI18n(language, SNAPSHOT_FORM_I18N);
	const [months, currency] = await Promise.all([
		getNewSnapshotMonths(language),
		getSavedBaseCurrency(language),
	]);

	if ("error" in months) return <ErrorToast message={months.error} />;

	if ("error" in currency) return <ErrorToast message={currency.error} />;

	const { months: open, opensOn } = months.openMonths;

	if (open.length === 0) {
		return (
			<div className="grid justify-items-start gap-3">
				{opensOn && (
					<Text variant="description">
						{t("closed").replace("{date}", formatDayMonth(opensOn, language))}
					</Text>
				)}
				<Link
					variant="button"
					href={getLanguageUrl(PAGE_URLS.SNAPSHOTS, language)}
				>
					{t("back_to_history")}
				</Link>
			</div>
		);
	}

	return (
		<NewSnapshotForm
			language={language}
			months={open}
			baseCurrency={currency.baseCurrency}
		/>
	);
};

export default NewSnapshot;
