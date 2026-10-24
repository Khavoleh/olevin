import { ErrorToast } from "@shared/components";
import { formatSnapshotMonth } from "@shared/helpers";
import { notFound } from "next/navigation";
import { parseMonthParam } from "../helpers/parse-month-param/parse-month-param";
import { getMonthAmounts } from "../loaders/get-month-amounts";
import { getSavedBaseCurrency } from "../loaders/get-saved-base-currency";
import EditEstimateForm from "./EditEstimateForm";
import EditSnapshotForm from "./EditSnapshotForm";

interface EditMonthProps {
	language: string;
	/** Month of the page path, as `2026-09`. */
	month: string;
}

/** Form of the saved snapshot or estimate of the month; a path that is not a saved month is not found. */
const EditMonth = async ({ language, month }: Readonly<EditMonthProps>) => {
	const parsed = parseMonthParam(month);

	if (!parsed) notFound();

	const [result, currency] = await Promise.all([
		getMonthAmounts(language, parsed.year, parsed.month),
		getSavedBaseCurrency(language),
	]);

	if ("error" in result) return <ErrorToast message={result.error} />;

	if ("error" in currency) return <ErrorToast message={currency.error} />;

	const { amounts } = result;
	const monthName = formatSnapshotMonth(parsed.year, parsed.month, language);

	if (amounts.kind === "Actual") {
		return (
			<EditSnapshotForm
				language={language}
				monthName={monthName}
				snapshot={amounts}
				baseCurrency={currency.baseCurrency}
			/>
		);
	}

	return (
		<EditEstimateForm
			language={language}
			monthName={monthName}
			estimate={amounts}
			baseCurrency={currency.baseCurrency}
		/>
	);
};

export default EditMonth;
