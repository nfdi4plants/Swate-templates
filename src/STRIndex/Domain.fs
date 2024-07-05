namespace STRIndex

open System.Text.Json

[<AutoOpen>]
module Domain =

    let jsonSerializerOptions = JsonSerializerOptions(WriteIndented = true)

    type SemVer() =
        member val Major = -1 with get,set
        member val Minor = -1 with get,set
        member val Patch = -1 with get,set
        member val PreRelease = "" with get,set
        member val BuildMetadata = "" with get,set

        override this.GetHashCode() =
            hash (
                this.Major, 
                this.Minor, 
                this.Patch, 
                this.PreRelease, 
                this.BuildMetadata
            )

        override this.Equals(other) =
            match other with
                | :? SemVer as s -> 
                    (
                        this.Major, 
                        this.Minor, 
                        this.Patch, 
                        this.PreRelease, 
                        this.BuildMetadata
                    ) = (
                        s.Major, 
                        s.Minor, 
                        s.Patch, 
                        s.PreRelease, 
                        s.BuildMetadata
                    )
                | _ -> false

        static member create (
            major: int,
            minor: int,
            patch: int,
            ?PreRelease: string,
            ?BuildMetadata: string
        ) =
            let tmp = SemVer(
                Major = major,
                Minor = minor,
                Patch = patch
            )
            PreRelease |> Option.iter (fun x -> tmp.PreRelease <- x)
            BuildMetadata |> Option.iter (fun x -> tmp.BuildMetadata <- x)
            tmp

        static member tryParse (version: string) =
            match version |> Globals.SEMVER_REGEX.Match |> fun m -> m.Success, m.Groups with
            | true, groups ->
                let major = groups.["major"].Value |> int
                let minor = groups.["minor"].Value |> int
                let patch = groups.["patch"].Value |> int
                let preRelease = groups.["prerelease"].Value
                let buildMetadata = groups.["buildmetadata"].Value
                Some(SemVer.create(major, minor, patch, preRelease, buildMetadata))
            | _ -> None

        static member toString (semVer: SemVer) =
            match (semVer.PreRelease, semVer.BuildMetadata) with
            | (pr, bm) when pr <> "" && bm <> "" -> $"{semVer.Major}.{semVer.Minor}.{semVer.Patch}-{pr}+{bm}"
            | (pr, bm) when pr <> "" && bm = "" -> $"{semVer.Major}.{semVer.Minor}.{semVer.Patch}-{pr}"
            | (pr, bm) when pr = "" && bm <> "" -> $"{semVer.Major}.{semVer.Minor}.{semVer.Patch}+{bm}"
            | _ -> $"{semVer.Major}.{semVer.Minor}.{semVer.Patch}"
            

    type Author() =
        member val FullName = "" with get,set
        member val Email = "" with get,set
        member val Affiliation = "" with get,set
        member val AffiliationLink = "" with get,set

        override this.GetHashCode() =
            hash (
                this.FullName, 
                this.Email, 
                this.Affiliation, 
                this.AffiliationLink
            )

        override this.Equals(other) =
            match other with
            | :? Author as a -> 
                (
                    this.FullName, 
                    this.Email, 
                    this.Affiliation, 
                    this.AffiliationLink
                ) = (
                    a.FullName, 
                    a.Email, 
                    a.Affiliation, 
                    a.AffiliationLink
                )
            | _ -> false

        static member create (
            fullName: string,
            ?Email: string,
            ?Affiliation: string,
            ?AffiliationLink: string
        ) =
            let tmp = Author(
                FullName = fullName
            )
            Email |> Option.iter (fun x -> tmp.Email <- x)
            Affiliation |> Option.iter (fun x -> tmp.Affiliation <- x)
            AffiliationLink |> Option.iter (fun x -> tmp.AffiliationLink <- x)

            tmp

    type OntologyAnnotation() =

        member val Name = "" with get,set
        member val TermSourceREF = "" with get,set
        member val TermAccessionNumber = "" with get,set

        override this.GetHashCode() =
            hash (
                this.Name, 
                this.TermSourceREF, 
                this.TermAccessionNumber
            )

        override this.Equals(other) =
            match other with
            | :? OntologyAnnotation as oa -> 
                (
                    this.Name, 
                    this.TermSourceREF, 
                    this.TermAccessionNumber
                ) = (
                    oa.Name, 
                    oa.TermSourceREF, 
                    oa.TermAccessionNumber
                )
            | _ -> false

        static member create (
            name: string,
            ?TermSourceRef: string,
            ?TermAccessionNumber: string
        ) =
            let tmp = new OntologyAnnotation(Name = name)
            TermSourceRef |> Option.iter (fun x -> tmp.TermSourceREF <- x)
            TermAccessionNumber |> Option.iter (fun x -> tmp.TermAccessionNumber <- x)
            tmp