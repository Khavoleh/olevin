import { Heading } from "@shared/components";
import { getI18n } from "@shared/helpers";
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
					<AccountSummary language={language} />
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
