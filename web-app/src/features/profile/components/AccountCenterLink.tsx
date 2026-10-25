"use client";

import { ArrowSquareOutIcon } from "@phosphor-icons/react";
import { useEffect, useRef, useTransition } from "react";
import { refreshAccount } from "../actions/refresh-account";
import SettingsLink from "./SettingsList/SettingsLink";

const OPENED_KEY = "account-center-opened";

/** Whether the Account Center was opened from this tab. */
const getIsOpened = () => {
	try {
		return sessionStorage.getItem(OPENED_KEY) === "true";
	} catch {
		return false;
	}
};

/** Remembers for this tab that the Account Center was opened. */
const markOpened = () => {
	try {
		sessionStorage.setItem(OPENED_KEY, "true");
	} catch {}
};

interface AccountCenterLinkProps {
	label: string;
	hint: string;
	href: string;
}

/** Opens the Logto Account Center in a new tab; once it was opened, every return to the profile shows the account as changed there. */
const AccountCenterLink = ({
	label,
	hint,
	href,
}: Readonly<AccountCenterLinkProps>) => {
	const isAwayRef = useRef(false);
	const [, startTransition] = useTransition();

	useEffect(() => {
		const leave = () => {
			if (getIsOpened()) isAwayRef.current = true;
		};

		const comeBack = () => {
			if (!isAwayRef.current || document.visibilityState !== "visible") {
				return;
			}

			isAwayRef.current = false;
			startTransition(refreshAccount);
		};

		const changeVisibility = () =>
			document.visibilityState === "hidden" ? leave() : comeBack();

		if (getIsOpened()) startTransition(refreshAccount);

		window.addEventListener("blur", leave);
		window.addEventListener("focus", comeBack);
		document.addEventListener("visibilitychange", changeVisibility);

		return () => {
			window.removeEventListener("blur", leave);
			window.removeEventListener("focus", comeBack);
			document.removeEventListener("visibilitychange", changeVisibility);
		};
	}, []);

	return (
		<SettingsLink
			label={label}
			hint={hint}
			href={href}
			isExternal
			isDocument
			icon={<ArrowSquareOutIcon size={18} />}
			onClick={markOpened}
		/>
	);
};

export default AccountCenterLink;
