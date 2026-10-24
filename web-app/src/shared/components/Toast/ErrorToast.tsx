"use client";

import { useEffect } from "react";
import showErrorToast from "./show-error-toast";

interface ErrorToastProps {
	message: string;
}

/** Shows an API error in a toast. */
const ErrorToast = ({ message }: Readonly<ErrorToastProps>) => {
	useEffect(() => {
		showErrorToast(message);
	}, [message]);

	return null;
};

export default ErrorToast;
