"use client";

import { useQuery } from "@tanstack/react-query";
import { getMe } from "./me/get-me";
import { ME_QUERY_KEY } from "./me/me-query-key";

interface MeDetailsProps {
	userIdLabel: string;
	loadingText: string;
	errorText: string;
}

/**
 * Reads the user from the query cache that the server filled, and keeps it fresh in the browser.
 */
const MeDetails = ({
	userIdLabel,
	loadingText,
	errorText,
}: Readonly<MeDetailsProps>) => {
	const { data, isError } = useQuery({
		queryKey: ME_QUERY_KEY,
		queryFn: getMe,
	});

	return (
		<p className="mt-2 min-h-6 break-all text-text-muted" aria-live="polite">
			{data && `${userIdLabel}: ${data.subject}`}
			{isError && <span className="text-danger">{errorText}</span>}
			{!data && !isError && loadingText}
		</p>
	);
};

export default MeDetails;
