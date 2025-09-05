export const qk = {
  me: ["auth", "me"] as const,
  languages: ["languages"] as const,
  localizationTable: (p: unknown) => ["localization-table", p] as const,
};
