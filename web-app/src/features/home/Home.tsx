import { Button, Card } from "@shared/components";
import { getI18n } from "@shared/helpers";
import { Suspense } from "react";
import { HOME_I18N } from "./home-i18n";
import MeInfo from "./MeInfo";
import MeInfoSkeleton from "./MeInfoSkeleton";
import { signOut } from "./sign-out";

interface HomeProps {
	language: string;
}

/** Home page of a signed-in user: their data and the sign-out button. */
const Home = ({ language }: Readonly<HomeProps>) => {
	const t = getI18n(language, HOME_I18N);

	return (
		<Card>
			<h1 className="font-semibold text-2xl">{t("title")}</h1>

			<Suspense fallback={<MeInfoSkeleton />}>
				<MeInfo language={language} />
			</Suspense>

			<form action={signOut} className="mt-6">
				<Button type="submit" variant="secondary" className="w-full">
					{t("sign_out")}
				</Button>
			</form>
		</Card>
	);
};

export default Home;
