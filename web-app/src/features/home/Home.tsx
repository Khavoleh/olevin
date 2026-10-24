import AdviceCard from "./components/AdviceCard/AdviceCard";
import CompositionCard from "./components/CompositionCard/CompositionCard";
import ContributionsCard from "./components/ContributionsCard/ContributionsCard";
import IndexCard from "./components/IndexCard/IndexCard";
import OverviewHeader from "./components/OverviewHeader/OverviewHeader";

interface HomeProps {
	language: string;
}

/** Home page of a signed-in user: the overview cards. */
const Home = ({ language }: Readonly<HomeProps>) => (
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
			</div>
		</div>
	</>
);

export default Home;
