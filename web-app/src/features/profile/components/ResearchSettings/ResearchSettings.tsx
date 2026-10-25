import { Suspense } from "react";
import ResearchConsentSetting from "./ResearchConsentSetting";

interface ResearchSettingsProps {
	language: string;
}

/** Consent to take part in the research and the way to revoke it; shown only once the settings say the user has consented. */
const ResearchSettings = ({ language }: Readonly<ResearchSettingsProps>) => (
	<Suspense>
		<ResearchConsentSetting language={language} />
	</Suspense>
);

export default ResearchSettings;
