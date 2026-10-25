import { Card, Heading } from "@shared/components";
import { getI18n } from "@shared/helpers";
import { Suspense } from "react";
import AccountSettings from "./components/AccountSettings";
import AccountSummary from "./components/AccountSummary";
import DataSettings from "./components/DataSettings";
import DocumentSettings from "./components/DocumentSettings";
import GeneralSettings from "./components/GeneralSettings";
import ReminderSettings from "./components/ReminderSettings";
import ResearchSettings from "./components/ResearchSettings";
import { PROFILE_I18N } from "./profile-i18n";

interface ProfileProps {
	language: string;
}

/** Profile page: the account card and the settings groups, from the language to the documents. */
const Profile = ({ language }: Readonly<ProfileProps>) => {
	const t = getI18n(language, PROFILE_I18N);

	return (
		<>
			<Heading level={1}>{t("title")}</Heading>

			<div className="@container">
				<div className="grid max-w-profile gap-5.5">
					<Suspense
						fallback={
							<Card className="flex items-center gap-3.5">
								<span
									aria-hidden
									className="size-13 flex-none rounded-full bg-skeleton"
								/>
								<span aria-hidden className="grid gap-1.5">
									<span className="h-5 w-44 rounded-md bg-skeleton" />
									<span className="h-4 w-36 rounded-md bg-skeleton" />
								</span>
							</Card>
						}
					>
						<AccountSummary language={language} />
					</Suspense>
					<GeneralSettings language={language} />
					<ReminderSettings language={language} />
					<ResearchSettings language={language} />
					<DataSettings language={language} />
					<AccountSettings language={language} />
					<DocumentSettings language={language} />
				</div>
			</div>
		</>
	);
};

export default Profile;
