import { PAGE_URLS } from "@shared/constants";
import {
	getApiTokenRSC,
	getI18n,
	getLanguageUrl,
	getQueryClient,
} from "@shared/helpers";
import { dehydrate, HydrationBoundary } from "@tanstack/react-query";
import { redirect } from "next/navigation";
import { HOME_I18N } from "./home-i18n";
import MeDetails from "./MeDetails";
import { fetchMe } from "./me/fetch-me";
import { ME_QUERY_KEY } from "./me/me-query-key";

interface MeInfoProps {
	language: string;
}

/**
 * Personal data: it needs the session, so it renders on every request and streams in behind `<Suspense>`.
 * The user is fetched here and handed to the browser's query cache, so the browser neither refetches nor shows a loader.
 */
const MeInfo = async ({ language }: Readonly<MeInfoProps>) => {
	const token = await getApiTokenRSC();

	if (!token) {
		redirect(getLanguageUrl(PAGE_URLS.SIGN_IN, language));
	}

	const queryClient = getQueryClient();

	// A failed prefetch is not an error here: the browser asks again and shows the error text if that fails too.
	await queryClient.prefetchQuery({
		queryKey: ME_QUERY_KEY,
		queryFn: () => fetchMe(token),
	});

	const t = getI18n(language, HOME_I18N);

	return (
		<HydrationBoundary state={dehydrate(queryClient)}>
			<MeDetails
				userIdLabel={t("user_id")}
				loadingText={t("loading")}
				errorText={t("error")}
			/>
		</HydrationBoundary>
	);
};

export default MeInfo;
