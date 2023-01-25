# SWATE templates

A collection of minimal information standard templates for Swate.

The templates in this repository are crawled by [Swobup](https://github.com/nfdi4plants/Swobup) and written into the production Swate database.

Anyone can access them via the [template search](https://nfdi4plants.github.io/Swate-docs/docs/UserDocs/Docs05-Templates.html) function.

---

## Contribution Guide

1. [Fork](https://docs.github.com/en/get-started/quickstart/fork-a-repo) this repository and [clone](https://docs.github.com/en/repositories/creating-and-managing-repositories/cloning-a-repository) your fork.
2. Create a new .xlsx file and open it with Excel.  
3. If not done already [install Swate](https://nfdi4plants.github.io/Swate-docs/docs/UserDocs/Docs01-Installing-Swate.html) and create an [Annotation Table](https://nfdi4plants.github.io/Swate-docs/docs/UserDocs/Docs02-Annotation-Table.html).
4. [Add building blocks](https://nfdi4plants.github.io/Swate-docs/docs/UserDocs/Docs03-Building-Blocks.html). If you are unsure of which columns to add:
    - keep the template as concise as possible
    - if you are adding a template with a specific endpoint repistory (ER) in mind, you may want to add columns that match the required fields of this ER
    - if you are adding a template because the research/methodical topic is missing, try to add columns that cover experimental procedures (as Parameters) and features of the sample (as Characteristics) that the experimenter would do when working on an experiment of that type
5. After you are done with the table, you have to add a SwateMetadataSheet: Click on the "Template Metdata" tab in **Swate Experts** and click "Create Metadata". A new worksheet will open. 
6. Type in a fitting name for the template (this will be the name that is displayed later for the user), as well as a short description, and your name into the author's list (you can also add your role into the author's role list and your email, phone, etc., if you like. These fields are optional)
![image](https://user-images.githubusercontent.com/47781170/146255531-97318a5f-cc34-420f-9474-0b09621ba65a.png)
6. As version, add "1.0.0" for new templates, or raise the version number if you update an existing template. The versioning follows the [SemVer](https://semver.org/) convention.
7. Go back to the sheet of the Swate table and find the Swate table's name under "Table Design" -> "Table Name" and write it into the Table field of the MetadataTable  
![image](https://user-images.githubusercontent.com/47781170/146319637-10a00303-7f9f-4d0c-9fb0-a457ed7863f1.png)
![image](https://user-images.githubusercontent.com/47781170/146319563-3144b549-02c7-4cf2-b20b-677deee99322.png)
8. Fill in Tags associated with the topic of this into the respective list. The same goes for ERs that this template should relate to
9. Add your organisation name to the "Organisation" field.
10. Close the file and give it an appropriate name (naming convention is work in progress).
11. Commit the changes on your file, push to your fork and start a [pull request](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/proposing-changes-to-your-work-with-pull-requests/about-pull-requests).

Well done! :tada: You created a new template. 

### Best Practises (Optional)

- Always try to think about in which order the experimenter in the lab will do their work. Try to match this chronological order from left to right. The normal order of the columns is: **Source Name** -> (all the Parameters and Characteristics in between in chronological order) -> **Source Name** -or- **Data File Name**. This step is optional and only meant to increase readability.
- Below the header you can add exemplary terms (as additional information for other Data Stewards) like here:  
  ![image](https://user-images.githubusercontent.com/47781170/146252236-0dd11621-76e9-4d28-b5fe-b495362a1cc5.png)
- Give the worksheet where the Swate table is located the same name as the file. This might not always be possible due to Excel worksheet name limitations.