import { Card, Text } from "@shared/components";
import { getI18n } from "@shared/helpers";
import { PROFILE_I18N } from "../profile-i18n";

const PREVIEW_EMAIL = "olena@example.com";

interface AccountSummaryProps {
	language: string;
}

/** Card of the signed-in account: its initial, email and how long it has kept snapshots. */
const AccountSummary = ({ language }: Readonly<AccountSummaryProps>) => {
	const t = getI18n(language, PROFILE_I18N);

	return (
		<Card className="flex items-center gap-3.5">
			<span
				aria-hidden
				className="flex size-13 flex-none items-center justify-center rounded-full bg-primary-soft font-bold text-primary text-xl uppercase"
			>
				{PREVIEW_EMAIL.charAt(0)}
			</span>
			<div className="grid min-w-0">
				<span className="truncate font-semibold text-lg">{PREVIEW_EMAIL}</span>
				<Text variant="hint">{t("summary_preview")}</Text>
			</div>
		</Card>
	);
};

export default AccountSummary;
