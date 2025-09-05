export default function About() {
  return (
    <div className="prose max-w-2xl">
      <h1>About this project</h1>
      <p>
        This application is a <strong>localization management tool</strong> for
        games. It allows teams to manage translation keys and their values
        across multiple languages.
      </p>
      <ul>
        <li>User authentication with login and registration.</li>
        <li>
          A table that shows all localization keys with translations per
          language.
        </li>
        <li>Inline editing of translations with autosave.</li>
        <li>Ability to add new keys and enable or disable languages.</li>
        <li>Admin panel for managing languages (create, edit, delete).</li>
      </ul>
      <p>
        The goal is to simplify the process of keeping game text consistent
        across different languages.
      </p>
    </div>
  );
}
