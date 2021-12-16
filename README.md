# SWATE templates

A collection of repository metadata templates for SWATE
- to fulfill end-point repository (e.g. EMBL-EBI, NCBI) requirements 
- in ISA-tab format
- built on ARC
- ontology linked and annotated using SWATE 

---

## For Data Stewards: How 2 create a Swate template

1. Use any tool you like that allows you to use Git (e.g. [GitHub Desktop](https://desktop.github.com/)), so that you can clone this repository:
    1. Click on "Clone a repository from  the internet"  
![image](https://user-images.githubusercontent.com/47781170/146229724-8b675647-9541-4522-8f25-d92e2810bedf.png)
    2. Click on the "URL" tab, paste the URL of this repo (https://github.com/nfdi4plants/SWATE_templates), and choose a path where the repository shall be. Click on clone  
    ![image](https://user-images.githubusercontent.com/47781170/146238392-8904d4bf-c515-4ab5-835b-70f1b760b39a.png)
    3. You can now open this folder in the file explorer  
    ![image](https://user-images.githubusercontent.com/47781170/146238760-9ebf10a7-7a39-43e8-b1a5-7428490c838e.png)
2. Start with creating a new .xlsx file:  
![image](https://user-images.githubusercontent.com/47781170/146222236-fa5c0952-95e1-4d29-ad25-ab23cac44901.png)
3. Give that file an appropriate name [[1]](#[1]-Naming-of-Swate-template-files), e.g. "2EXT06_XenobioticSubstances.xlsx" if you are interested in creating a template for the extraction of toxins or alike from a sample
4. Open that file in MS Excel
5. [Open Swate](https://github.com/nfdi4plants/Swate/wiki/Starting-Swate) and use it to [create an annotation table](https://github.com/nfdi4plants/Swate/wiki/Creating-Annotation-tables)
6. [Add the columns](https://github.com/nfdi4plants/Swate/wiki/Adding-Building-Blocks) you like to add. If you are unsure of which columns to add:
    - if you are adding a template with a specific endpoint repistory (ER) in mind, you may want to add columns that match the required fields of this ER
    - if you are adding a template because the research/methodical topic is missing, try to add columns that cover experimental procedures (as Parameters) and features of the sample (as Characteristics) that the experimenter would do when working on an experiment of that type
7. Always try to think about in which order the experimenter in the lab will do their work. Try to match this chronological order from left to right. The normal order of the columns is: **Source Name** -> (all the Parameters and Characteristics in between in chronological order) -> **Source Name** -or- **Data File Name**
8. Below the header you can add exemplary terms (as additional information for other Data Stewards) like here in _2EXT02_Protein.xlsx_:  
  ![image](https://user-images.githubusercontent.com/47781170/146252236-0dd11621-76e9-4d28-b5fe-b495362a1cc5.png)
9. Give the worksheet where the Swate table is located the same name as the file:  
![image](https://user-images.githubusercontent.com/47781170/146319903-679a77a1-399c-4b25-b4f7-28c040b7766f.png)
10. Once all desired columns are added, you may want to give them additional information, e.g. how important each of them is compared to each other. For this, open Swate Experts:
![image](https://user-images.githubusercontent.com/47781170/146252813-7712a53d-f5c2-441e-8e41-6eeb86ee5d88.png)
    1. Click on the tab "Checklist Editor" and, if no table representation appears automatically, on "Update table representation"
![image](https://user-images.githubusercontent.com/47781170/146321929-cf72a0d7-b053-4c46-81d1-7a8a7f22ec77.png)
    2. Here you can set the importance of each column as well as its content type. After you are done, hit "Add checklist to workbook"
![image](https://user-images.githubusercontent.com/47781170/146322079-2e195c9b-78cf-44f3-9158-9e6199d90db8.png)
12. After you are done with the table, you have to add a SwateMetadataSheet: Click on the "Template Metdata" tab and next on "Create Metadata"  
![image](https://user-images.githubusercontent.com/47781170/146253890-9a5afbfb-ea08-491c-b2ab-87e5d5d2968e.png)
12. Annotate the sheet as follows:
    1. Type in a fitting name for the template (this will be the name that is displayed later for the user), as well as a nice (but short) description, and your name into the author's list (you can also add your role into the author's role list and your email, phone, etc., if you like. These fields are optional)
![image](https://user-images.githubusercontent.com/47781170/146255531-97318a5f-cc34-420f-9474-0b09621ba65a.png)
    2. As version, add "1.0.0" for new templates, or raise the version number if you update an existing template. The versioning follows the [SemVer](https://semver.org/) convention  
![image](https://user-images.githubusercontent.com/47781170/146319464-952fb007-487d-44da-9731-d8e092d80700.png)
    3. Go back to the sheet of the Swate table and find the Swate table's name under "Table Design" -> "Table Name" and write it into the Table field of the MetadataTable  
![image](https://user-images.githubusercontent.com/47781170/146319637-10a00303-7f9f-4d0c-9fb0-a457ed7863f1.png)
![image](https://user-images.githubusercontent.com/47781170/146319563-3144b549-02c7-4cf2-b20b-677deee99322.png)
    4. Fill in Tags associated with the topic of this into the respective list. The same goes for ERs that this template should relate to
![image](https://user-images.githubusercontent.com/47781170/146320122-9f650df1-e5e1-4c59-8fc8-78038eaaf97e.png)
    5. You can also fill the remaining fields, but they are optional. "Docslink" shall provide a link to the documentation of this template (WIP 🚧) and "Organisation" is the name of the organisation you are associated with
13. Since you are done with the basic stuff, save the table and close Excel
14. Use the ER sheet creation tool (WIP 🚧) to create ER sheets from the Swate table of a template that you can annotate (of course, only if your template tackles any ERs). Open the file again in Excel. There are now additional worksheets (one per ER):
![image](https://user-images.githubusercontent.com/47781170/146320767-e822c9cf-571e-4dc0-8e80-172dec325558.png)
    1. On the left are the names of the columns' headers. "TermSourceRef", "Ontology", and "TAN" refer to the respective ontological terms. They get created automatically via the tool
    2. "Content type (validation)" provides information for filling out the Swate Checklist Editor (s. point 10). This field is **optional**
    3. "Note during templating" refers to things that you (or another template creater) had while writing the template and which are / could be important. This field is **optional**
    4. "Target term" tells which field in the ER the field of this column maps to (e.g. the column "Parameter [cleavage agent name]" might map to "Protease used" of a fictitious ER). This field is **mandatory if "Requirement" is set to "m" or "o"**
    5. "Instruction" is the information of the ER for the mapped field (means: What does this ER's field describe?). This field is **optional**
    6. "Requirement" tells if the column's values _must_ be mapped ("m" = mandatory), _can_ be mapped ("o" = optional), or _cannot_ be mapped ("n" = no requirement). This field is **mandatory**
    7. "Value" describes which kind of value the field has _in the ER_ (**not** in the Swate table!). It can be "cv" (= controlled vocabulary), "s" (= string, meaning text) or "d" (= double, meaning number). This field is **mandatory if "Requirement is set to "m" or "o"**
    8. "Additional information" is, well, additional information that you want to provide to other Data Stewards (or your later self) regarding this ER's field (for additional information about this column of the Swate table, use "Notes during templating")
    9. "Review comments" is a field left for potential reviewers who want to comment on the annotations of this line. As a reviewer, you can add things to consider here
15. Once you are done, save the file again and close Excel
16. In GitHub Desktop (or whatever tool you are using to work with Git) you can now commit the changes that you applied to the repository. Write mandatory a commit message (here "Create 2EXT06_XenobioticSubstances.xlsx" is inferred by default but you can individualize the message) and, optionally, a description. Click on "Commit to main"
![image](https://user-images.githubusercontent.com/47781170/146326619-05637e4a-40e5-4631-93c1-644c47fe5c50.png)
17. Well done! You created a new template and added it to the Swate_templates repository. It will be added shortly after to the database automatically via Swobup

##### [1] Naming of Swate template files:
- Take for example "1SPL01_plants.xlsx"
- The first number (in our example: "1") **defines for the order** of the protocols: Templates with 1 are first, succeeded by those with 2 and so on...
- The three letters after that (in our example: "SPL") **describe the kind of the template** and are linked to the first number:
  - "1SPL" stands for "Sample preparation": What does the experimenter do with their sample in the beginning? What features (organism, tissue, ...) does it have? What do the experimenter have to do first, in order to extract the interesting parts of the sample later?
  - "2EXT" stands for "Extraction": How do the experimenter extract the parts of his original sample they are interested in? What methods do they use?
  - "3ASY" stands for "Assay": After the experimenter extracted the interesting parts of their original sample: Which measurements do they do? With which parameters?
  - "4COM" stands for "Computation": The experimenter collected the information about their sample in the step before and now has raw data files. What software with which parameters do they use to process the raw data? What kind of analysis do they use?
- After this, an underscore with a text-based **name of this specific template** follows (in our example: "_plants")
