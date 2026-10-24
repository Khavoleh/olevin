import { Button, Card, Heading } from "@shared/components";
import { getI18n } from "@shared/helpers";
import AdviceCard from "./components/AdviceCard/AdviceCard";
import CompositionCard from "./components/CompositionCard/CompositionCard";
import ContributionsCard from "./components/ContributionsCard/ContributionsCard";
import IndexCard from "./components/IndexCard/IndexCard";
import OverviewHeader from "./components/OverviewHeader/OverviewHeader";
import { HOME_I18N } from "./i18n/home-i18n";
import { signOut } from "./sign-out";

interface HomeProps {
	language: string;
}

/** Home page of a signed-in user: the overview cards and the sign-out button. */
const Home = ({ language }: Readonly<HomeProps>) => {
	const t = getI18n(language, HOME_I18N);

	return (
		<>
			<OverviewHeader language={language} />

			<div className="@container">
				<div className="grid @4xl:grid-cols-12 grid-cols-1 gap-3 md:gap-4">
					<IndexCard
						language={language}
						className="@4xl:order-1 @4xl:col-span-5"
					/>
					<AdviceCard
						language={language}
						className="@4xl:order-3 @4xl:col-span-7"
					/>
					<CompositionCard
						language={language}
						className="@4xl:order-2 @4xl:col-span-7"
					/>
					<ContributionsCard
						language={language}
						className="@4xl:order-4 @4xl:col-span-5"
					/>
					<Card className="@4xl:order-5 @4xl:col-span-full">
						<Heading level={2}>{t("trend")}</Heading>
					</Card>
				</div>
			</div>

			<form action={signOut}>
				<Button type="submit" variant="secondary">
					{t("sign_out")}
				</Button>
			</form>
		</>
	);
};

export default Home;
