import { getI18n } from "@shared/helpers";
import { SNAPSHOTS_I18N } from "../snapshots-i18n";

interface EstimateTagProps {
	language: string;
}

/** Chip that marks an estimated month from onboarding. */
const EstimateTag = ({ language }: Readonly<EstimateTagProps>) => {
	const t = getI18n(language, SNAPSHOTS_I18N);

	return (
		<span className="inline-flex w-fit rounded-full bg-surface-sunken px-2 py-0.5 font-semibold text-fg-muted text-xs uppercase tracking-label">
			{t("estimated")}
		</span>
	);
};

export default EstimateTag;
