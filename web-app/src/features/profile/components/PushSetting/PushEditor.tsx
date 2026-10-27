"use client";

import { showErrorToast } from "@shared/components";
import { getI18n } from "@shared/helpers";
import { useEffect, useState, useTransition } from "react";
import { removePushSubscription } from "../../actions/remove-push-subscription";
import { savePushSubscription } from "../../actions/save-push-subscription";
import { decodeBase64Url } from "../../helpers/decode-base64-url/decode-base64-url";
import { getDeviceName } from "../../helpers/get-device-name/get-device-name";
import {
	getPushSupport,
	type PushSupport,
} from "../../helpers/get-push-support/get-push-support";
import { PROFILE_I18N } from "../../profile-i18n";
import type { PushDevice } from "../../push-device";
import SettingsSwitch from "../SettingsList/SettingsSwitch";
import PushDeviceRow from "./PushDeviceRow";

interface PushEditorProps {
	language: string;
	devices: PushDevice[];
	vapidPublicKey: string;
}

const HINT_KEYS: Record<PushSupport, string> = {
	available: "push_hint",
	denied: "push_hint_denied",
	install: "push_hint_install",
	unsupported: "push_hint_unsupported",
};

const readPushSupport = () =>
	getPushSupport({
		hasServiceWorker: "serviceWorker" in navigator,
		hasPushManager: "PushManager" in window,
		hasNotification: "Notification" in window,
		isIos:
			/iPhone|iPad|iPod/.test(navigator.userAgent) ||
			(navigator.userAgent.includes("Macintosh") &&
				navigator.maxTouchPoints > 1),
		isStandalone: window.matchMedia("(display-mode: standalone)").matches,
		permission: "Notification" in window ? Notification.permission : "default",
	});

const getBrowserSubscription = async () =>
	(await navigator.serviceWorker.ready).pushManager.getSubscription();

/** Push switch of this browser and the browsers of the user that get the push. */
const PushEditor = ({
	language,
	devices,
	vapidPublicKey,
}: Readonly<PushEditorProps>) => {
	const t = getI18n(language, PROFILE_I18N);
	const [support, setSupport] = useState<PushSupport>();
	const [endpoint, setEndpoint] = useState<string>();
	const [isPending, startTransition] = useTransition();
	const current = devices.find((device) => device.endpoint === endpoint);

	useEffect(() => {
		const found = readPushSupport();

		setSupport(found);

		if (found === "available" || found === "denied") {
			getBrowserSubscription()
				.then((subscription) => setEndpoint(subscription?.endpoint))
				.catch(() => undefined);
		}
	}, []);

	const turnOn = () =>
		startTransition(async () => {
			const permission = await Notification.requestPermission();

			if (permission !== "granted") {
				setSupport(readPushSupport());
				return;
			}

			try {
				const registration = await navigator.serviceWorker.ready;
				const subscription =
					(await registration.pushManager.getSubscription()) ??
					(await registration.pushManager.subscribe({
						userVisibleOnly: true,
						applicationServerKey: decodeBase64Url(vapidPublicKey),
					}));
				const { keys } = subscription.toJSON();
				const error = await savePushSubscription(language, {
					endpoint: subscription.endpoint,
					p256dh: keys?.p256dh ?? "",
					auth: keys?.auth ?? "",
					deviceName: getDeviceName(
						navigator.userAgent,
						t("push_device_unknown"),
					),
				});

				if (error) showErrorToast(error);
				else setEndpoint(subscription.endpoint);
			} catch {
				showErrorToast(t("push_error"));
			}
		});

	const remove = (device: PushDevice) =>
		startTransition(async () => {
			const error = await removePushSubscription(language, device.id);

			if (error) {
				showErrorToast(error);
				return;
			}

			if (device.endpoint === endpoint) {
				await (await getBrowserSubscription())
					?.unsubscribe()
					.catch(() => false);
				setEndpoint(undefined);
			}
		});

	return (
		<>
			<SettingsSwitch
				label={t("push")}
				hint={t(HINT_KEYS[support ?? "available"])}
				isSelected={Boolean(current)}
				isDisabled={support !== "available" || isPending}
				onChange={(isOn) => {
					if (isOn) turnOn();
					else if (current) remove(current);
				}}
			/>
			{devices.map((device) => (
				<PushDeviceRow
					key={device.id}
					language={language}
					device={device}
					isCurrent={device.endpoint === endpoint}
					isDisabled={isPending}
					onRemove={() => remove(device)}
				/>
			))}
		</>
	);
};

export default PushEditor;
