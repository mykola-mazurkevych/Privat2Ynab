# Privat2Ynab

Privat2Ynab is a personal CLI utility that imports statement exports from PrivatBank (Privat24) into YNAB.

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE).

PrivatBank/Privat24 is a Ukrainian banking service that allows exporting account statements.
YNAB (You Need A Budget) is a budgeting app with an API for importing transactions.

This tool helps map exported statement files to YNAB accounts, then applies optional category and payee rules before sending transactions to YNAB.

## How it works

1. Configure at least one YNAB plan.
2. Configure account mapping between a statement file name and a YNAB account.
3. Optionally configure category and payee rules.
4. Put statement files into the `input` folder next to the executable.
5. Run the app with no command arguments to import files.

## Publish

```powershell
dotnet publish src/Privat2Ynab.Console/Privat2Ynab.Console.csproj -c Release --self-contained true -p:PublishSingleFile=true -o <PUBLISH_DIR>
```

Published executable location:

`<PUBLISH_DIR>\Privat2Ynab.exe`

## Run

1. Open PowerShell or Command Prompt and move to the publish directory:

```powershell
cd <PUBLISH_DIR>
```

2. Run command mode:

```powershell
.\Privat2Ynab.exe <command>
```

3. Run import mode (default root command):

```powershell
.\Privat2Ynab.exe
```

## Statement files location

Put exported statement files into `<PUBLISH_DIR>\input` (next to `Privat2Ynab.exe`).

Example directory layout:

```text

<PUBLISH_DIR>\
	Privat2Ynab.exe
	input\
		card-statement-july.xlsx
		card-statement-august.xlsx
```

The `--file-name` value used in `accounts add` must match the actual file name in `input` (case-insensitive).

## Available commands

### Plans

1. List plans

```powershell
.\Privat2Ynab.exe plans list
```

2. Add plan

```powershell
.\Privat2Ynab.exe plans add --ynab-id <GUID> --token <YNAB_PERSONAL_ACCESS_TOKEN>
```

3. Delete plan

```powershell
.\Privat2Ynab.exe plans delete --id <PLAN_ID>
```

### Accounts

1. List accounts

```powershell
.\Privat2Ynab.exe accounts list
```

2. Add account mapping

```powershell
.\Privat2Ynab.exe accounts add --plan-id <PLAN_ID> --ynab-id <YNAB_ACCOUNT_GUID> --file-name <STATEMENT_FILE_NAME>
```

3. Delete account

```powershell
.\Privat2Ynab.exe accounts delete --id <ACCOUNT_ID>
```

### Category rules

1. List category rules

```powershell
.\Privat2Ynab.exe category-rules list
```

2. Add category rule

```powershell
.\Privat2Ynab.exe category-rules add --plan-id <PLAN_ID> --memo <TEXT> --match-type <Exact|StartsWith|EndsWith|Contains> --category-group-name <GROUP_NAME> --category-name <CATEGORY_NAME>
```

3. Delete category rule

```powershell
.\Privat2Ynab.exe category-rules delete --id <RULE_ID>
```

### Payee rules

1. List payee rules

```powershell
.\Privat2Ynab.exe payee-rules list
```

2. Add payee rule

```powershell
.\Privat2Ynab.exe payee-rules add --plan-id <PLAN_ID> --memo <TEXT> --match-type <Exact|StartsWith|EndsWith|Contains> --payee-name <PAYEE_NAME>
```

3. Delete payee rule

```powershell
.\Privat2Ynab.exe payee-rules delete --id <RULE_ID>
```

## Notes

1. Account mapping uses statement file name as the key.
2. `match-type` values are `Exact`, `StartsWith`, `EndsWith`, `Contains`.
3. The database is created and migrations are applied automatically at startup.