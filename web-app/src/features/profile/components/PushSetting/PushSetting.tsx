import { ErrorToast } from "@shared/components";
import { getVapidPublicKey } from "../../helpers/get-vapid-public-key";
import { getProfileSettings } from "../../loaders/get-profile-settings";
import { getPushSubscriptions } from "../../loaders/get-push-subscriptions";
import type { PushDevice } from "../../push-device";
import PushEditor from "./PushEditor";

interface PushSettingProps {
	language: string;
}

/** Push switch of this browser and the list of the browsers that get the push. */
const PushSetting = async ({ language }: Readonly<PushSettingProps>) => {
	const [result, settings] = await Promise.all([
		getPushSubscriptions(language),
		getProfileSettings(language),
	]);

	if ("error" in result) return <ErrorToast message={result.error} />;

	const added = new Intl.DateTimeFormat(language, {
		day: "numeric",
		month: "long",
		year: "numeric",
		timeZone: "settings" in settings ? settings.settings.timezone : "UTC",
	});
	const devices: PushDevice[] = result.subscriptions.map((subscription) => ({
		id: subscription.id,
		endpoint: subscription.endpoint,
		name: subscription.deviceName,
		added: added.format(new Date(subscription.createdAt)),
	}));

	return (
		<PushEditor
			language={language}
			devices={devices}
			vapidPublicKey={getVapidPublicKey()}
		/>
	);
};

export default PushSetting;
