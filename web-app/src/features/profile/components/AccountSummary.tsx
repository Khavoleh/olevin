import { Card, Text } from "@shared/components";
import { getI18n } from "@shared/helpers";
import { getProfileSettings } from "../loaders/get-profile-settings";
import { PROFILE_I18N } from "../profile-i18n";

interface AccountSummaryProps {
	language: string;
}

/** Card of the signed-in account: its initial, username, email and when it was created. */
const AccountSummary = async ({ language }: Readonly<AccountSummaryProps>) => {
	const t = getI18n(language, PROFILE_I18N);
	const result = await getProfileSettings(language);

	if ("error" in result) return null;

	const { username, email, createdAt, timezone } = result.settings;
	const name = username ?? email ?? "";
	const joined = new Intl.DateTimeFormat(language, {
		day: "numeric",
		month: "long",
		year: "numeric",
		timeZone: timezone,
	}).format(new Date(createdAt));
	const details = [username && email, t("joined").replace("{date}", joined)];

	return (
		<Card className="flex items-center gap-3.5">
			<span
				aria-hidden
				className="flex size-13 flex-none items-center justify-center rounded-full bg-primary-soft font-bold text-primary text-xl uppercase"
			>
				{name.charAt(0)}
			</span>
			<div className="grid min-w-0">
				<span className="truncate font-semibold text-lg">{name}</span>
				<Text variant="hint">{details.filter(Boolean).join(" · ")}</Text>
			</div>
		</Card>
	);
};

export default AccountSummary;
