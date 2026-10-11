import Profile from "@features/profile";

interface ProfilePageProps {
	params: Promise<{ lang: string }>;
}

/** Route of the profile page. */
const ProfilePage = async ({ params }: Readonly<ProfilePageProps>) => {
	const { lang } = await params;

	return <Profile language={lang} />;
};

export default ProfilePage;
