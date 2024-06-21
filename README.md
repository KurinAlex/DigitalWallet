# DigitalWallet

**DigitalWallet** is a minimalistic and user-friendly payment system.

After registration, clients can create their own virtual financial wallet, which offers a wide range of functionalities, such as:
- *Depositing* funds from a bank account
- *Transferring* funds to other wallets
- *Paying* for services from any available company

Additionally, clients can view their entire *transaction history*.

## Summary

Each client can have only *one* digital wallet. 

After its creation, the client will receive a **wallet ID**, which is a [UUID](https://en.wikipedia.org/wiki/Universally_unique_identifier) for their digital wallet.

### Financial Operations

Let's dive into the details of each financial operation!

#### Deposit Funds

**Deposit Funds** allows clients to transfer funds from their bank account to their digital wallet.

On the Deposit Funds page, clients can specify the amount they want to deposit.

After clicking the *'Deposit'* button, they will be redirected to the [Stripe Payment Element](https://docs.stripe.com/payments/payment-element), where they can enter their bank account information.

#### Transfer Funds

**Transfer Funds** allows clients to transfer funds from their digital wallet to another client's digital wallet.

On the Transfer Funds page, clients can specify the email or wallet ID of the recipient, the amount they want to transfer, and an optional description.

By clicking the *'Transfer'* button, the transaction will be completed almost instantly.

#### Make a Payment

**Make a Payment** allows clients to pay directly to a wide range of companies for any service.

On the Make a Payment page, clients can choose the company, specify the service, and the amount to pay.

By clicking the *'Pay'* button, the payment will be completed.

### User Types

There are two types of users:

- **Clients:** Can perform financial operations with their wallets, view their transactions, and manage their accounts.
- **Admins:** Can manage companies that are used for payment operations. A default main admin is created at application start.

## For Developers

This solution utilizes a variety of technologies and frameworks:

- [ASP.NET Core](https://learn.microsoft.com/aspnet/core/introduction-to-aspnet-core) and [Razor Pages](https://learn.microsoft.com/en-us/aspnet/core/razor-pages) for creating the web application.
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/) for ORM.
- [ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity) for authentication/authorization logic.
- [jQuery](https://jquery.com/) for easier JavaScript development.

Additionally, it uses several third-party services and libraries:

- [Stripe](https://stripe.com/) for real bank account transactions.
- [SendGrid](https://sendgrid.com/) for email sending.
- [Bootstrap](https://getbootstrap.com/) for managing front-end styles.
- [DataTables](https://datatables.net/) for advanced front-end tables.
- [Toastr](https://codeseven.github.io/toastr/) for front-end non-blocking notifications.
- [qrcode.js](https://davidshimjs.github.io/qrcodejs/) for front-end QR-code generation.

### Architecture and Structure

The application follows an MVC/MVVM architecture with the following projects:

- **DigitalWallet:** Web app project containing Razor pages (Views) and their view models (Controllers/View Models). It also contains all JavaScript scripts.
- **DigitalWallet.Data:** Class library project as the Model part of the app.
- **DigitalWallet.Services:** Class library project as the Service Tier.

### Full Set-Up Process

#### Prerequisites

- **Visual Studio:** [Download here](https://visualstudio.microsoft.com/downloads/)
- **.NET SDK:** [Download here](https://dotnet.microsoft.com/download)
- **SendGrid account and API key:** [Tutorial](https://www.twilio.com/docs/sendgrid/for-developers/sending-email/api-getting-started)
- **Stripe account and API key:** [Tutorial](https://docs.stripe.com/api)
- **SQL Database:** [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads)

#### 1. Install

First, clone the DigitalWallet [git repository](https://github.com/KurinAlex/DigitalWallet) to your local machine.

You can refer to [this guide](https://learn.microsoft.com/en-us/visualstudio/version-control/git-clone-repository) for instructions.

#### 2. Configure Secrets

All necessary data for the program is stored in app secrets.

You can find information and guidance on local app secrets in [this tutorial](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets).

The content of the `secrets.json` file must have the following structure:

```json
{
  "EmailSenderOptions": {
    "SenderEmail": "<email of confirmation letters sender>",
    "SendGridKey": "<your SendGrid API key>"
  },
  "StripeOptions": {
    "StripeKey": "<your Stripe API key>"
  },
  "AdminAccountOptions": {
    "Email": "<email of default admin account>",
    "Password": "<password of default admin account>"
  },
  "ConnectionStrings": {
    "Database": "<your database connection string>"
  }
}
```

`AdminAccountOptions` is used to configure the default admin user, which will be seeded into the database when the application starts.

#### 3. Configure Local Database

In order to create your local database, you should run the following command in Visual Studio CLI:

`
update-database
`

This will generate and apply SQL scripts for tables creation and configuration.

You can find more on working with database in Entity Framework on [Migrations Overview](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations) page.

#### 4. Enjoy!

Congratulations! You're set up and can work with this solution on your own.

### Azure Deployment

#### 1. Connect to Azure Key Vault

This application uses Azure Key Vault for secret storage in production scenarios.

Follow [this tutorial](https://learn.microsoft.com/en-us/visualstudio/azure/vs-key-vault-add-connected-service) to connect to Azure Key Vault.

Note that the JSON file hierarchy must be represented with the `--` divider in secrets' names.

Store the Azure Key Vault connection string in the `VaultUri` variable in AppSettings.

Azure Key Vault must contain the following secrets:
- `EmailSenderOptions--SenderEmail`
- `EmailSenderOptions--SendGridKey`
- `StripeOptions--StripeKey`
- `AdminAccountOptions--Email`
- `AdminAccountOptions--Password`
- `ConnectionStrings--Database`

#### 2. Connect to Azure SQL Database

Refer to this [tutorial](https://learn.microsoft.com/en-us/aspnet/core/tutorials/publish-to-azure-webapp-using-vs#deploy-the-app-to-azure) to create and connect to an Azure SQL database.

Set the database connection string location to the `ConnectionStrings--Database` variable in Azure Key Vault.

#### 3. Deploy

Deploy your app to Azure by clicking the 'Publish' button after all settings are configured.

## Final Notes

This app was created with significant influence and assistance from the [.NET Core MVC - The Complete Guide](https://www.udemy.com/course/complete-aspnet-core-21-course) course.
